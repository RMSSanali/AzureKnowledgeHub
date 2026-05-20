import { useEffect, useMemo, useState } from 'react';
import {
  API_BASE_URL,
  clearStoredToken,
  createLearningResource,
  deleteLearningResource,
  getLearningResourceById,
  getLearningResources,
  getProfile,
  getStoredToken,
  loginUser,
  registerUser,
  searchLearningResources,
  setStoredToken,
  updateLearningResource,
} from './api/client.js';
import SearchPanel from './components/SearchPanel.jsx';
import ResourceCard from './components/ResourceCard.jsx';
import ResourceDetail from './components/ResourceDetail.jsx';
import LoginPanel from './components/LoginPanel.jsx';
import ProfileCard from './components/ProfileCard.jsx';
import AdminCreateResourceForm from './components/AdminCreateResourceForm.jsx';
import AdminEditResourceForm from './components/AdminEditResourceForm.jsx';

const defaultSearch = {
  query: '',
  certificationPath: '',
  difficultyLevel: '',
  tag: '',
  pageNumber: 1,
  pageSize: 5,
};

function App() {
  const [resources, setResources] = useState([]);
  const [searchFilters, setSearchFilters] = useState(defaultSearch);
  const [searchResult, setSearchResult] = useState(null);
  const [selectedResource, setSelectedResource] = useState(null);
  const [loading, setLoading] = useState(true);
  const [detailLoading, setDetailLoading] = useState(false);
  const [error, setError] = useState('');
  const [currentUser, setCurrentUser] = useState(null);
  const [authLoading, setAuthLoading] = useState(false);
  const [authError, setAuthError] = useState('');
  const [activeView, setActiveView] = useState('home');
  const [editingResource, setEditingResource] = useState(null);
  const [adminActionMessage, setAdminActionMessage] = useState('');

  useEffect(() => {
    loadResources();
  }, []);

  useEffect(() => {
    if (getStoredToken()) {
      loadProfile();
    }
  }, []);

  const visibleResources = useMemo(() => {
    return searchResult ? searchResult.items : resources;
  }, [resources, searchResult]);

  async function loadResources() {
    setLoading(true);
    setError('');

    try {
      const data = await getLearningResources();
      setResources(data);
      setSearchResult(null);
      setSelectedResource(data[0] || null);
    } catch {
      setError(`Could not load resources. Make sure the API is running at ${API_BASE_URL}.`);
    } finally {
      setLoading(false);
    }
  }

  async function loadProfile() {
    setAuthLoading(true);
    setAuthError('');

    try {
      const profile = await getProfile();
      setCurrentUser(profile);
    } catch {
      clearStoredToken();
      setCurrentUser(null);
      setAuthError('Saved login expired or could not be loaded. Please login again.');
    } finally {
      setAuthLoading(false);
    }
  }

  async function handleLogin(credentials) {
    setAuthLoading(true);
    setAuthError('');

    try {
      const auth = await loginUser(credentials);
      setStoredToken(auth.token);
      const profile = await getProfile();
      setCurrentUser(profile);
    } catch {
      clearStoredToken();
      setCurrentUser(null);
      setAuthError('Login failed. Check the username/email and password.');
    } finally {
      setAuthLoading(false);
    }
  }

  async function handleRegisterLearner(user) {
    setAuthLoading(true);
    setAuthError('');

    try {
      await registerUser(user);
    } finally {
      setAuthLoading(false);
    }
  }

  function handleLogout() {
    clearStoredToken();
    setCurrentUser(null);
    setAuthError('');
    setEditingResource(null);
    setAdminActionMessage('');
  }

  async function handleCreateResource(resource) {
    await createLearningResource(resource);
    await loadResources();
  }

  async function handleEditResource(resource) {
    setError('');
    setAdminActionMessage('');

    try {
      const detail = await getLearningResourceById(resource.learningResourceId);
      setEditingResource(detail);
      setActiveView('account');
    } catch {
      setError('Could not load resource for editing.');
    }
  }

  async function handleUpdateResource(id, resource) {
    await updateLearningResource(id, resource);
    await loadResources();
    setEditingResource(null);
    setAdminActionMessage('Learning resource updated and list refreshed.');
  }

  async function handleDeleteResource(resource) {
    const confirmed = window.confirm('Are you sure you want to delete this resource?');
    if (!confirmed) {
      return;
    }

    setError('');
    setAdminActionMessage('');

    try {
      await deleteLearningResource(resource.learningResourceId);
      await loadResources();
      setEditingResource(null);
      setAdminActionMessage('Learning resource deleted and list refreshed.');
    } catch {
      setError('Could not delete resource. Check admin permissions and try again.');
    }
  }

  async function runSearch(filters) {
    setLoading(true);
    setError('');

    try {
      const data = await searchLearningResources(filters);
      setSearchFilters(filters);
      setSearchResult(data);
      setSelectedResource(null);
    } catch {
      setError(`Search failed. Make sure the API is running at ${API_BASE_URL}.`);
    } finally {
      setLoading(false);
    }
  }

  function handleSearch(filters) {
    runSearch({ ...filters, pageNumber: 1 });
  }

  function handleReset() {
    setSearchFilters(defaultSearch);
    loadResources();
  }

  async function handleSelect(resource) {
    setDetailLoading(true);
    setError('');

    try {
      const detail = await getLearningResourceById(resource.learningResourceId);
      setSelectedResource(detail);
    } catch {
      setSelectedResource(resource);
      setError('Could not load full resource details. Showing summary information instead.');
    } finally {
      setDetailLoading(false);
    }
  }

  function changePage(nextPage) {
    runSearch({ ...searchFilters, pageNumber: nextPage });
  }

  const canPageBack = searchResult && searchResult.pageNumber > 1;
  const canPageForward = searchResult && searchResult.pageNumber < searchResult.totalPages;

  return (
    <main className="app-shell">
      <section className="intro-band">
        <div>
          <p className="eyebrow">Thesis prototype</p>
          <h1>AzureKnowledgeHub</h1>
          <p className="intro-copy">
            Browse and search Azure certification learning resources from the local ASP.NET Core API.
          </p>
        </div>
        <div className="header-side">
          <div className="api-status">
            <span>API</span>
            <strong>{API_BASE_URL}</strong>
          </div>
          <LoginPanel
            user={currentUser}
            loading={authLoading}
            error={authError}
            onLogin={handleLogin}
            onLogout={handleLogout}
            onRegister={handleRegisterLearner}
          />
        </div>
      </section>

      <nav className="view-tabs" aria-label="Main views">
        <button
          type="button"
          className={activeView === 'home' ? 'active' : ''}
          onClick={() => setActiveView('home')}
        >
          Home
        </button>
        <button
          type="button"
          className={activeView === 'account' ? 'active' : ''}
          onClick={() => setActiveView('account')}
        >
          Profile / Admin
        </button>
      </nav>

      {activeView === 'home' && (
        <>
          <SearchPanel initialFilters={searchFilters} onSearch={handleSearch} onReset={handleReset} />

          {adminActionMessage && <div className="message success-box">{adminActionMessage}</div>}
          {error && <div className="message error-message">{error}</div>}
          {loading && <div className="message">Loading learning resources...</div>}

          {!loading && (
            <section className="content-grid">
              <div className="resource-list">
                <div className="section-heading">
                  <div>
                    <p className="eyebrow">Resources</p>
                    <h2>{searchResult ? 'Search results' : 'All learning resources'}</h2>
                  </div>
                  <span className="count-badge">
                    {searchResult ? `${searchResult.totalCount} found` : `${resources.length} total`}
                  </span>
                </div>

                {visibleResources.length === 0 ? (
                  <div className="empty-state">No resources found.</div>
                ) : (
                  <div className="cards">
                    {visibleResources.map((resource) => (
                      <ResourceCard
                        key={resource.learningResourceId}
                        resource={resource}
                        isAdmin={currentUser?.role === 'Admin'}
                        isSelected={selectedResource?.learningResourceId === resource.learningResourceId}
                        onDelete={handleDeleteResource}
                        onEdit={handleEditResource}
                        onSelect={handleSelect}
                      />
                    ))}
                  </div>
                )}

                {searchResult && (
                  <div className="pagination">
                    <button type="button" onClick={() => changePage(searchResult.pageNumber - 1)} disabled={!canPageBack}>
                      Previous
                    </button>
                    <span>
                      Page {searchResult.pageNumber} of {searchResult.totalPages || 1}
                    </span>
                    <button
                      type="button"
                      onClick={() => changePage(searchResult.pageNumber + 1)}
                      disabled={!canPageForward}
                    >
                      Next
                    </button>
                  </div>
                )}
              </div>

              <ResourceDetail resource={selectedResource} loading={detailLoading} />
            </section>
          )}
        </>
      )}

      {activeView === 'account' && (
        <section className="account-view">
          {adminActionMessage && <div className="message success-box">{adminActionMessage}</div>}
          {currentUser ? (
            <div className="account-tools">
              <ProfileCard user={currentUser} />
              {currentUser.role === 'Admin' ? (
                <>
                  <AdminEditResourceForm
                    resource={editingResource}
                    onCancel={() => setEditingResource(null)}
                    onUpdateResource={handleUpdateResource}
                  />
                  <AdminCreateResourceForm onCreateResource={handleCreateResource} />
                </>
              ) : (
                <div className="admin-notice">Admin tools are only available for admin users.</div>
              )}
            </div>
          ) : (
            <div className="message">Please log in to view profile.</div>
          )}
        </section>
      )}
    </main>
  );
}

export default App;
