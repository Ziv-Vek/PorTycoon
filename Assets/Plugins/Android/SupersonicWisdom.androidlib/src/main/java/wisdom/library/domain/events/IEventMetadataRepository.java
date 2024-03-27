package wisdom.library.domain.events;

public interface IEventMetadataRepository {

    void put(String eventMetadataJson);
    String get();
    void update(String eventMetadataJson);
}
