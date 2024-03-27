package wisdom.library.data.repository.datasource;

import wisdom.library.data.framework.local.ConversionDataLocalApi;

public class ConversionDataLocalDataSource {
    private ConversionDataLocalApi mApi;

    public ConversionDataLocalDataSource(ConversionDataLocalApi api) {
        mApi = api;
    }

    public String getConversionData() {
        return mApi.getConversionData();
    }
}
