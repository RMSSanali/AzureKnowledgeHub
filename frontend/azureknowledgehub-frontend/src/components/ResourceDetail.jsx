function ResourceDetail({ resource, loading }) {
  if (loading) {
    return (
      <aside className="detail-panel">
        <div className="message">Loading details...</div>
      </aside>
    );
  }

  if (!resource) {
    return (
      <aside className="detail-panel">
        <p className="eyebrow">Details</p>
        <h2>Select a resource</h2>
        <p className="muted-text">Choose a learning resource to view its content and tags.</p>
      </aside>
    );
  }

  return (
    <aside className="detail-panel">
      <p className="eyebrow">Selected resource</p>
      <h2>{resource.title}</h2>
      <div className="detail-meta">
        <span>{resource.certificationPath}</span>
        <span>{resource.difficultyLevel}</span>
        <span>{resource.categoryName}</span>
      </div>
      <p className="summary-text">{resource.summary}</p>
      {resource.content && <p className="content-text">{resource.content}</p>}
      <div className="tag-row detail-tags">
        {(resource.tags || []).map((tag) => (
          <span key={tag}>{tag}</span>
        ))}
      </div>
      <dl className="date-list">
        <div>
          <dt>Created</dt>
          <dd>{new Date(resource.createdAt).toLocaleDateString()}</dd>
        </div>
        <div>
          <dt>Published</dt>
          <dd>{resource.isPublished ? 'Yes' : 'No'}</dd>
        </div>
      </dl>
    </aside>
  );
}

export default ResourceDetail;
