package wisdom.library.domain.mapper;

public abstract class Mapper<T1, T2> {

    public abstract T2 map(T1 value);
    public abstract T1 reverse(T2 value);
}
