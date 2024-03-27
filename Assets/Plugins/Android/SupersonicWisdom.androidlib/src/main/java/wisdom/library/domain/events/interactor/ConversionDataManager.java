package wisdom.library.domain.events.interactor;

import wisdom.library.domain.events.IConversionDataRepository;

public class ConversionDataManager implements IConversionDataManager {

    private IConversionDataRepository mRepository;

    public ConversionDataManager(IConversionDataRepository repository) {
        mRepository = repository;
    }

    @Override
    public String getConversionData() {
        return mRepository.getConversionData();
    }
}
