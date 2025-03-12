namespace ProvaPub.Models
{
    public class Pagination<T> where T : class
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public bool HasNext { get; set; }

        public int TotalPages
        {
            get
            {
                if (PageSize > 0)
                {
                    int num = TotalCount / PageSize;
                    int num2 = ((TotalCount % PageSize > 0) ? 1 : 0);
                    return num + num2;
                }

                return 0;
            }
        }

        public static Pagination<T> Create(IQueryable<T> query, int currentPage, int pageSize = 0)
        {
            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            var pagination = new Pagination<T>();
            pagination.TotalCount = query.Count();
            pagination.PageSize = pageSize;
            pagination.CurrentPage = currentPage;
            pagination.Items = query.Skip(pageSize * (currentPage - 1)).Take(pageSize).ToList();
            pagination.HasNext = currentPage * pageSize < pagination.TotalCount;
            return pagination;
        }
    }
}
