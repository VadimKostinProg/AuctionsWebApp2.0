using AutoMapper;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Application.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public CategoriesService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var specification = new SpecificationBuilder<Category>()
                .With(x => x.IsDeleted == false)
                .OrderBy(x => x.Name, Enums.SortDirection.ASC)
                .Build();

            var categories = await _repository.GetAsync<Category>(specification);

            return categories.Select(x => this._mapper.Map<CategoryDTO>(x)).ToList();
        }

        public async Task CreateNewCategoryAsync(CreateCategoryDTO category)
        {
            // Validating category
            if (category is null)
                throw new ArgumentNullException("Category is null.");

            if (string.IsNullOrEmpty(category.Name))
                throw new ArgumentNullException("Name cannot be blank.");

            if (string.IsNullOrEmpty(category.Description))
                throw new ArgumentNullException("Description cannot be blank.");

            if (await _repository.AnyAsync<Category>(x => x.Name == category.Name && !x.IsDeleted))
                throw new ArgumentException("Category with the same name already exists.");

            // Creating new category
            var categoryToCreate = this._mapper.Map<Category>(category);
            categoryToCreate.Id = Guid.NewGuid();

            await _repository.AddAsync(categoryToCreate);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            // Validating category
            var category = await _repository.FirstOrDefaultAsync<Category>(x => x.Id == id && !x.IsDeleted);

            if (category is null)
                throw new KeyNotFoundException("Category with such id does not exist.");

            // Deleting category
            category.IsDeleted = true;
            await _repository.UpdateAsync(category);
        }

        public async Task<ListModel<CategoryDTO>> GetCategoriesListAsync(SpecificationsDTO specifications)
        {
            if (specifications is null)
                throw new ArgumentNullException("Specifications are null.");

            var specification = this.GetSpecification(specifications);

            var categories = await _repository.GetAsync<Category>(specification);

            var totalCount = specification.Predicate is null ? 
                await _repository.CountAsync<Category>() :
                await _repository.CountAsync<Category>(specification.Predicate);

            var totalPages = (long)Math.Ceiling((double)totalCount / specifications.PageSize);

            var list = new ListModel<CategoryDTO>()
            {
                List = categories.Select(x => this._mapper.Map<CategoryDTO>(x)).ToList(),
                TotalPages = totalPages
            };

            return list;
        }

        // Method for creating ISpecification for CategorySpecificationsDTO
        private ISpecification<Category> GetSpecification(SpecificationsDTO specifications)
        {
            var builder = new SpecificationBuilder<Category>();

            builder.With(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                builder.With(x => x.Name.Contains(specifications.SearchTerm));

            if (!string.IsNullOrEmpty(specifications.SortField))
            {
                switch (specifications.SortField)
                {
                    case "id":
                        builder.OrderBy(x => x.Id, specifications.SortDirection ?? Enums.SortDirection.ASC);
                        break;
                    case "name":
                        builder.OrderBy(x => x.Name, specifications.SortDirection ?? Enums.SortDirection.ASC);
                        break;
                }
            }

            builder.WithPagination(specifications.PageSize, specifications.PageNumber);

            return builder.Build();
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(Guid id)
        {
            var category = await _repository.FirstOrDefaultAsync<Category>(x => x.Id == id && !x.IsDeleted);

            if (category is null)
                throw new KeyNotFoundException("Category with such Id does not exist.");

            return this._mapper.Map<CategoryDTO>(category);
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDTO category)
        {
            // Validating category
            if (category is null)
                throw new ArgumentNullException("Category is null.");

            var existantCategory = await _repository.GetByIdAsync<Category>(category.Id);

            if (existantCategory is null)
                throw new KeyNotFoundException("Category with such id does not exist.");

            if (string.IsNullOrEmpty(category.Name))
                throw new ArgumentNullException("Name cannot be blank.");

            if (string.IsNullOrEmpty(category.Description))
                throw new ArgumentNullException("Description cannot be blank.");

            if (await _repository.AnyAsync<Category>(x => x.Id != category.Id &&
                                                          x.Name == category.Name &&
                                                          x.IsDeleted == false))
                throw new ArgumentException("Category with the same name already exists.");

            // Updating category
            existantCategory = this._mapper.Map<Category>(category);
            await _repository.UpdateAsync(existantCategory);
        }
    }
}
