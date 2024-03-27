package wisdom.library.data.repository;

import wisdom.library.data.repository.datasource.ConversionDataLocalDataSource;
import wisdom.library.domain.events.IConversionDataRepository;

public class ConversionDataRepository implements IConversionDataRepository {

    private ConversionDataLocalDataSource mDataSource;

    public ConversionDataRepository(ConversionDataLocalDataSource dataSource) {
        mDataSource = dataSource;
    }

    @Override
    public String getConversionData() {
        return mDataSource.getConversionData();
    }
}
