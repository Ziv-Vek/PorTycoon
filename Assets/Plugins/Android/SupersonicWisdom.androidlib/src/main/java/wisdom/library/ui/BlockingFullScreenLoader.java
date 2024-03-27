package wisdom.library.ui;

import android.app.Activity;
import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Movie;
import android.os.Handler;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.widget.FrameLayout;

import wisdom.library.util.SdkLogger;

import java.io.InputStream;
import java.lang.ref.WeakReference;

public class BlockingFullScreenLoader {

    private static final String TAG = BlockingFullScreenLoader.class.getSimpleName();
    private final String imageFilePath;
    private final int percentageFromScreenWidth;
    private LoaderView loaderView;

    private interface BooleanPredicate<T> {
        boolean test(T currentView);
    }

    WeakReference<Activity> activityWeakReference;
    public BlockingFullScreenLoader(Activity activity, final String imageFilePath, final int percentageFromScreenWidth) {
        activityWeakReference = new WeakReference<>(activity);
        this.imageFilePath = imageFilePath;
        this.percentageFromScreenWidth = percentageFromScreenWidth;
    }

    public boolean show(final Handler unityHandler) {
        if (loaderView != null && loaderView.getParent() != null) return true;

        if (activityWeakReference == null) return false;
        final Activity activity = activityWeakReference.get();
        if (activity == null) return false;
        final View decorView = activity.getWindow().getDecorView();
        if (decorView == null) return false;

        View result = UiUtils.findViewByPredicate(decorView, new BooleanPredicate<View>() {
            @Override
            public boolean test(View currentView) {
                return currentView instanceof FrameLayout;
            }
        });

        if (result == null) {
            SdkLogger.error(TAG, "Failed to find Unity's FrameLayout");
            return false;
        }

        SdkLogger.log(result);

        final FrameLayout unityContainer = (FrameLayout) result;
        unityContainer.post(new Runnable() {
            @Override
            public void run() {
                if (loaderView == null) {
                    loaderView = new LoaderView(unityContainer.getContext(), unityHandler, imageFilePath, percentageFromScreenWidth);

                    loaderView.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View view) {
                            // Ignore all taps in order to block user interaction
                        }
                    });
                }

                loaderView.setAlpha(0);
                unityContainer.addView(loaderView, new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.MATCH_PARENT));
                loaderView.animate().alpha(1).setDuration(100).start();
            }
        });

        return true;
    }

    public boolean hide() {
        if (loaderView == null) return false;
        if (loaderView.getParent() == null) return false;
        final ViewParent superview = loaderView.getParent();
        if (!(superview instanceof ViewGroup)) return false;

        loaderView.post(new Runnable() {
            @Override
            public void run() {
                loaderView.animate().alpha(0).setDuration(100).withEndAction(new Runnable() {
                    @Override
                    public void run() {
                        ((ViewGroup) superview).removeView(loaderView);
                    }
                }).start();
            }
        });

        return true;
    }

    private static class UiUtils {
        private static View searchSubviewsTree(int level, View currentView, BooleanPredicate<View> predicate) {
            if (predicate.test(currentView)) return currentView;

            if (currentView instanceof ViewGroup) {
                View found = null;
                ViewGroup currentViewGroup = ((ViewGroup)currentView);
                int childCount = currentViewGroup.getChildCount();
                for (int i = 0; i < childCount; i++) {
                    View child = currentViewGroup.getChildAt(i);
                    found = searchSubviewsTree(level + 1, child, predicate);
                    if (found != null) return found;
                }
            }

            return null;
        }

        public static View findViewByPredicate(View rootView, BooleanPredicate<View> predicate) {
            return searchSubviewsTree(0, rootView, predicate);
        }
    }

    private static class LoaderView extends FrameLayout {
        public LoaderView(Context context, Handler unityHandler, String imageFilePath, int percentageFromScreenWidth) {
            super(context);

            setBackgroundColor(Color.parseColor("#AA000000"));
            GifPlayerView gifPlayerView = new GifPlayerView(getContext(), unityHandler, imageFilePath, percentageFromScreenWidth);
            addView(gifPlayerView, new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.MATCH_PARENT));
        }
    }

    private static class GifPlayerView extends View {
        private Movie movie;
        private long mMovieStart;

        public GifPlayerView(final Context context, Handler unityHandler, final String imageFilePath, final int percentageFromScreenWidth) {
            super(context);

            setBackgroundColor(Color.parseColor("#00000000"));

            unityHandler.post(new Runnable() {
                @Override
                public void run() {
                    try {
                        String relativePath = extractSuffixFromString(imageFilePath, "SupersonicWisdom/");
                        InputStream inputStream = context.getResources().getAssets().open(relativePath);

                        // This is a time consuming operation and it can be done on a BG thread. Running it on "Unity Main Thread" to still avoid sync issues (due to Coroutine operations).
                        movie = Movie.decodeStream(inputStream);
                        float ratio = (float) movie.width() / (float) getWidth();
                        float fullWidthScale = 1 / ratio;
                        float percentage = ((float) percentageFromScreenWidth) / 100f;
                        final float widthScale = fullWidthScale * percentage;

                        post(new Runnable() {
                            @Override
                            public void run() {
                                setScaleX(widthScale); // set to 100% of the width
                                setScaleY(widthScale);
                                // Tell the UI thread that the "movie" is ready for painting.
                                invalidate();
                            }
                        });
                    } catch (Exception e) {
                        SdkLogger.error(TAG, e);
                    }
                }
            });
        }

        private String extractSuffixFromString(String absolute, String token) {
            if (absolute == null) return absolute;
            if (token == null) return absolute;

            String[] components = absolute.split(token);
            if (components.length != 2) return absolute;

            return token + components[1];
        }

        @Override
        protected void onDraw(Canvas canvas) {
            canvas.drawColor(0x00CCCCCC); // set background color

            long now = android.os.SystemClock.uptimeMillis();
            if (mMovieStart == 0) {   // first time
                mMovieStart = now;
            }

            if (movie != null) {
                int duration = movie.duration();
                if (duration == 0) {
                    duration = 1000;
                }

                int relTime = (int)((now - mMovieStart) % duration);
                movie.setTime(relTime);

                movie.draw(canvas,
                        (getWidth() - movie.width()) >> 1,
                        (getHeight() - movie.height()) >> 1); // shift right (>>) will divide it to half

                invalidate();
            }
        }
    }
}
