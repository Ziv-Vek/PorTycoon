package wisdom.library.data.framework.network.core;

import wisdom.library.data.framework.network.request.WisdomRequestExecutorTask;

import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

public class WisdomNetworkDispatcher {

    private Executor mExecutor;

    public WisdomNetworkDispatcher() {
        mExecutor = Executors.newFixedThreadPool(5);
    }

    public void dispatch(WisdomRequestExecutorTask task) {
        mExecutor.execute(task);
    }
}
