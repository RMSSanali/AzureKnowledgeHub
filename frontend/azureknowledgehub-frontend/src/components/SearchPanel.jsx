import { useEffect, useState } from 'react';

function SearchPanel({ initialFilters, onSearch, onReset }) {
  const [filters, setFilters] = useState(initialFilters);

  useEffect(() => {
    setFilters(initialFilters);
  }, [initialFilters]);

  function updateFilter(name, value) {
    setFilters((current) => ({ ...current, [name]: value }));
  }

  function submit(event) {
    event.preventDefault();
    onSearch(filters);
  }

  return (
    <form className="search-panel" onSubmit={submit}>
      <label>
        Search
        <input
          value={filters.query}
          onChange={(event) => updateFilter('query', event.target.value)}
          placeholder="storage, identity, monitoring..."
        />
      </label>

      <label>
        Certification
        <select
          value={filters.certificationPath}
          onChange={(event) => updateFilter('certificationPath', event.target.value)}
        >
          <option value="">All</option>
          <option value="AZ-900">AZ-900</option>
          <option value="AZ-104">AZ-104</option>
        </select>
      </label>

      <label>
        Difficulty
        <select
          value={filters.difficultyLevel}
          onChange={(event) => updateFilter('difficultyLevel', event.target.value)}
        >
          <option value="">All</option>
          <option value="Beginner">Beginner</option>
          <option value="Intermediate">Intermediate</option>
        </select>
      </label>

      <label>
        Tag
        <input
          value={filters.tag}
          onChange={(event) => updateFilter('tag', event.target.value)}
          placeholder="Security"
        />
      </label>

      <label>
        Page size
        <select value={filters.pageSize} onChange={(event) => updateFilter('pageSize', Number(event.target.value))}>
          <option value={5}>5</option>
          <option value={10}>10</option>
        </select>
      </label>

      <div className="search-actions">
        <button type="submit">Search</button>
        <button type="button" className="secondary-button" onClick={onReset}>
          Reset
        </button>
      </div>
    </form>
  );
}

export default SearchPanel;
