import { useEffect, useState } from 'react';

function toForm(resource) {
  return {
    title: resource?.title || '',
    summary: resource?.summary || '',
    content: resource?.content || '',
    certificationPath: resource?.certificationPath || 'AZ-900',
    difficultyLevel: resource?.difficultyLevel || 'Beginner',
    categoryId: String(resource?.categoryId || 1),
    tags: (resource?.tags || []).join(', '),
    isPublished: resource?.isPublished ?? true,
  };
}

function AdminEditResourceForm({ resource, onCancel, onUpdateResource }) {
  const [form, setForm] = useState(toForm(resource));
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    setForm(toForm(resource));
    setError('');
  }, [resource]);

  if (!resource) {
    return null;
  }

  function updateField(event) {
    const { name, type, checked, value } = event.target;
    setForm((current) => ({
      ...current,
      [name]: type === 'checkbox' ? checked : value,
    }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setLoading(true);
    setError('');

    const payload = {
      title: form.title.trim(),
      summary: form.summary.trim(),
      content: form.content.trim(),
      certificationPath: form.certificationPath,
      difficultyLevel: form.difficultyLevel,
      categoryId: Number(form.categoryId),
      isPublished: form.isPublished,
      tags: form.tags
        .split(',')
        .map((tag) => tag.trim())
        .filter(Boolean),
    };

    try {
      await onUpdateResource(resource.learningResourceId, payload);
    } catch {
      setError('Could not update resource. Check required fields and admin permissions.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="admin-edit-panel" aria-label="Admin edit learning resource">
      <p className="eyebrow">Admin tools</p>
      <h2>Edit learning resource</h2>
      <form className="admin-create-form" onSubmit={handleSubmit}>
        <label>
          Title
          <input name="title" value={form.title} onChange={updateField} required />
        </label>
        <label>
          Summary
          <textarea name="summary" value={form.summary} onChange={updateField} rows="3" required />
        </label>
        <label>
          Content
          <textarea name="content" value={form.content} onChange={updateField} rows="4" required />
        </label>
        <div className="form-row">
          <label>
            Certification
            <select name="certificationPath" value={form.certificationPath} onChange={updateField}>
              <option value="AZ-900">AZ-900</option>
              <option value="AZ-104">AZ-104</option>
            </select>
          </label>
          <label>
            Difficulty
            <select name="difficultyLevel" value={form.difficultyLevel} onChange={updateField}>
              <option value="Beginner">Beginner</option>
              <option value="Intermediate">Intermediate</option>
            </select>
          </label>
          <label>
            Category ID
            <input name="categoryId" type="number" min="1" value={form.categoryId} onChange={updateField} required />
          </label>
        </div>
        <label>
          Tags
          <input name="tags" value={form.tags} onChange={updateField} placeholder="Azure, Security, AZ-900" />
        </label>
        <label className="checkbox-label">
          <input name="isPublished" type="checkbox" checked={form.isPublished} onChange={updateField} />
          Published
        </label>
        <div className="form-actions">
          <button type="submit" disabled={loading}>
            {loading ? 'Saving...' : 'Save changes'}
          </button>
          <button type="button" className="secondary-button" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </form>
      {error && <p className="login-error">{error}</p>}
    </section>
  );
}

export default AdminEditResourceForm;
