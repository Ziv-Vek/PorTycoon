package wisdom.library.util;

import java.lang.reflect.Method;

/**
 * This class wraps an Android task and mocks a completion callback.
 * Instead of registering itself as a listener, it polls the task and awaits for completion.
 *
 * @param <ResultType>
 */
public class SwAndroidTaskWrapper<ResultType> {
    public interface IResultExtractor<T> {
        T getResult(Object taskResultObject) throws ReflectiveOperationException;
    }

    private final Object taskObject;
    private final SwCallback<ResultType> callback;
    private final IResultExtractor<ResultType> resultExtractor;

    public SwAndroidTaskWrapper(Object taskObject, IResultExtractor<ResultType> resultExtractor, SwCallback<ResultType> callback) {
        this.taskObject = taskObject;
        this.callback = callback;
        this.resultExtractor = resultExtractor;
        
        waitForResult();
    }

    private void waitForResult() {
        if (callback == null) return;
        if (taskObject == null) {
            callback.onDone(null);
            return;
        }

        SwUtils.bgThreadHandler().postDelayed(new Runnable() {
            @Override
            public void run() {
                ResultType result = null;

                try {
                    Class<?> appSetIdClientClass = Class.forName("com.google.android.gms.tasks.Task");
                    Method isTaskCompletedGetterMethod = appSetIdClientClass.getMethod("isComplete");
                    Object isTaskCompletedObject = isTaskCompletedGetterMethod.invoke(taskObject);
                    if (isTaskCompletedObject instanceof Boolean && ((Boolean) isTaskCompletedObject)) {
                        Method resultGetterMethod = appSetIdClientClass.getMethod("getResult");
                        Object taskResultObject = resultGetterMethod.invoke(taskObject);
                        result = resultExtractor.getResult(taskResultObject);

                        callback.onDone(result);
                    } else {
                        waitForResult();
                    }
                } catch (Throwable e) {
                    SdkLogger.error("SwAndroidTaskWrapper", e);
                    callback.onDone(result);
                }
            }
        }, 200);
    }
}