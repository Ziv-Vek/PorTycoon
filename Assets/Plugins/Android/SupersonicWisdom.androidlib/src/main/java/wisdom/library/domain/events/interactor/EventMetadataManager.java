package wisdom.library.domain.events.interactor;

import wisdom.library.domain.events.IEventMetadataRepository;

public class EventMetadataManager implements IEventMetadataManager {

    private IEventMetadataRepository mEventMetadataRepository;

    public EventMetadataManager(IEventMetadataRepository repository) {
        mEventMetadataRepository = repository;
    }

    @Override
    public void set(String metadataJson) {
        mEventMetadataRepository.put(metadataJson);
    }

    @Override
    public void update(String metadataJson) {
        mEventMetadataRepository.update(metadataJson);
    }

    @Override
    public String get() {
        return mEventMetadataRepository.get();
    }
}
