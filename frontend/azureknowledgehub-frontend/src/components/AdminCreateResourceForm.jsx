import { useState } from 'react';

const initialForm = {
  title: '',
  summary: '',
  content: '',
  certificationPath: 'AZ-900',
  difficultyLevel: 'Beginner',
  categoryId: '1',
  tags: '',
  isPublished: true,
};

function AdminCreateResourceForm({ onCreateResource }) {
  const [form, setForm] = useState(initialForm);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

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
    setMessage('');
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
      await onCreateResource(payload);
      setForm(initialForm);
      setMessage('Learning resource created and list refreshed.');
    } catch {
      setError('Could not create resource. Check required fields and admin permissions.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="admin-create-panel" aria-label="Admin create learning resource">
      <p className="eyebrow">Admin tools</p>
      <h2>Create learning resource</h2>
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
        <button type="submit" disabled={loading}>
          {loading ? 'Creating...' : 'Create resource'}
        </button>
      </form>
      {message && <p className="success-message">{message}</p>}
      {error && <p className="login-error">{error}</p>}
    </section>
  );
}

export default AdminCreateResourceForm;
