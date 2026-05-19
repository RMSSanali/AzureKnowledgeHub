function ResourceCard({ resource, isAdmin, isSelected, onDelete, onEdit, onSelect }) {
  return (
    <article className={`resource-card ${isSelected ? 'selected' : ''}`}>
      <button className="resource-card-main" type="button" onClick={() => onSelect(resource)}>
        <div className="card-topline">
          <span>{resource.certificationPath}</span>
          <span>{resource.difficultyLevel}</span>
        </div>
        <h3>{resource.title}</h3>
        <p>{resource.summary}</p>
        <div className="meta-row">
          <span>{resource.categoryName}</span>
        </div>
        <div className="tag-row">
          {(resource.tags || []).map((tag) => (
            <span key={tag}>{tag}</span>
          ))}
        </div>
      </button>
      {isAdmin && (
        <div className="card-actions">
          <button type="button" onClick={() => onEdit(resource)}>
            Edit
          </button>
          <button type="button" className="danger-button" onClick={() => onDelete(resource)}>
            Delete
          </button>
        </div>
      )}
    </article>
  );
}

export default ResourceCard;
