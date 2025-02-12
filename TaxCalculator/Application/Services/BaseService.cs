namespace TaxCalculator.Application.Services;

public abstract class BaseService<T> where T : class {
    protected readonly ILogger<T> _logger;
    protected readonly IMapper _mapper;

    public BaseService(ILogger<T> logger, IMapper mapper) {
        _logger = logger;
        _mapper = mapper;
    }
}
