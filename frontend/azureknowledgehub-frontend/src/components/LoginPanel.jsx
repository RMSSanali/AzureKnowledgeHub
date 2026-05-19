import { useEffect, useState } from 'react';

function LoginPanel({ user, loading, error, onLogin, onLogout }) {
  const [credentials, setCredentials] = useState({
    usernameOrEmail: '',
    password: '',
  });

  useEffect(() => {
    if (user) {
      setCredentials({
        usernameOrEmail: '',
        password: '',
      });
    }
  }, [user]);

  function updateField(event) {
    const { name, value } = event.target;
    setCredentials((current) => ({
      ...current,
      [name]: value,
    }));
  }

  function handleSubmit(event) {
    event.preventDefault();
    onLogin(credentials);
  }

  if (user) {
    return (
      <section className="login-panel" aria-label="Signed in user">
        <p className="eyebrow">Signed in</p>
        <h2>{user.displayName || user.username}</h2>
        <div className="user-facts">
          <span>{user.username}</span>
          <span>{user.role}</span>
        </div>
        <button type="button" className="secondary-button" onClick={onLogout}>
          Logout
        </button>
      </section>
    );
  }

  return (
    <section className="login-panel" aria-label="Login">
      <p className="eyebrow">Authentication</p>
      <h2>Login</h2>
      <form className="login-form" onSubmit={handleSubmit}>
        <label>
          Username or email
          <input
            name="usernameOrEmail"
            value={credentials.usernameOrEmail}
            onChange={updateField}
            autoComplete="username"
            required
          />
        </label>
        <label>
          Password
          <input
            name="password"
            type="password"
            value={credentials.password}
            onChange={updateField}
            autoComplete="current-password"
            required
          />
        </label>
        <button type="submit" disabled={loading}>
          {loading ? 'Logging in...' : 'Login'}
        </button>
      </form>
      {error && <p className="login-error">{error}</p>}
    </section>
  );
}

export default LoginPanel;
