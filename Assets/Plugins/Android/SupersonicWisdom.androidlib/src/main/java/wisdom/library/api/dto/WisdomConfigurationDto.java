package wisdom.library.api.dto;

public class WisdomConfigurationDto {
    public String subdomain;
    public int readTimeout;
    public int connectTimeout;
    public int initialSyncInterval;
    public boolean isLoggingEnabled;
    public String streamingAssetsFolderPath;
    public String blockingLoaderResourceRelativePath;
    public int blockingLoaderViewportPercentage;
}
