package wisdom.library.data.framework.watchdog;

import android.app.Activity;

import wisdom.library.domain.watchdog.dto.ActivityDto;

public class ActivityDtoMapper {

    public static ActivityDto map(Activity activity) {
        ActivityDto dto = new ActivityDto();
        dto.activityName = activity.getLocalClassName();
        return dto;
    }
}
