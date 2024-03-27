package wisdom.library.domain.events.interactor;

public interface IEventMetadataManager {

    void set(String metadataJson);
    void update(String metadataJson);
    String get();
}
