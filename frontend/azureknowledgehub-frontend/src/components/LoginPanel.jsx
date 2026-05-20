import { useEffect, useState } from 'react';

function LoginPanel({ user, loading, error, onLogin, onLogout, onRegister }) {
  const [credentials, setCredentials] = useState({
    usernameOrEmail: '',
    password: '',
  });
  const [registerMode, setRegisterMode] = useState(false);
  const [registration, setRegistration] = useState({
    displayName: '',
    username: '',
    email: '',
    password: '',
  });
  const [registerMessage, setRegisterMessage] = useState('');
  const [registerError, setRegisterError] = useState('');

  useEffect(() => {
    if (user) {
      setCredentials({
        usernameOrEmail: '',
        password: '',
      });
      setRegisterMode(false);
      setRegisterMessage('');
      setRegisterError('');
    }
  }, [user]);

  function updateField(event) {
    const { name, value } = event.target;
    setCredentials((current) => ({
      ...current,
      [name]: value,
    }));
  }

  function updateRegistrationField(event) {
    const { name, value } = event.target;
    setRegistration((current) => ({
      ...current,
      [name]: value,
    }));
  }

  function handleSubmit(event) {
    event.preventDefault();
    setRegisterMessage('');
    onLogin(credentials);
  }

  async function handleRegisterSubmit(event) {
    event.preventDefault();
    setRegisterMessage('');
    setRegisterError('');

    try {
      await onRegister(registration);
      setCredentials({
        usernameOrEmail: registration.username || registration.email,
        password: '',
      });
      setRegistration({
        displayName: '',
        username: '',
        email: '',
        password: '',
      });
      setRegisterMode(false);
      setRegisterMessage('Learner account created. You can now log in.');
    } catch {
      setRegisterError('Registration failed. Check that username and email are not already used.');
    }
  }

  function showRegisterForm() {
    setRegisterMode(true);
    setRegisterMessage('');
    setRegisterError('');
  }

  function showLoginForm() {
    setRegisterMode(false);
    setRegisterError('');
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

  if (registerMode) {
    return (
      <section className="login-panel" aria-label="Register learner">
        <p className="eyebrow">Authentication</p>
        <h2>Register learner</h2>
        <form className="login-form" onSubmit={handleRegisterSubmit}>
          <label>
            Display name
            <input
              name="displayName"
              value={registration.displayName}
              onChange={updateRegistrationField}
              autoComplete="name"
              required
            />
          </label>
          <label>
            Username
            <input
              name="username"
              value={registration.username}
              onChange={updateRegistrationField}
              autoComplete="username"
              required
            />
          </label>
          <label>
            Email
            <input
              name="email"
              type="email"
              value={registration.email}
              onChange={updateRegistrationField}
              autoComplete="email"
              required
            />
          </label>
          <label>
            Password
            <input
              name="password"
              type="password"
              value={registration.password}
              onChange={updateRegistrationField}
              autoComplete="new-password"
              required
            />
          </label>
          <button type="submit" disabled={loading}>
            {loading ? 'Creating...' : 'Create learner account'}
          </button>
        </form>
        <button type="button" className="text-button" onClick={showLoginForm}>
          Back to login
        </button>
        {registerError && <p className="login-error">{registerError}</p>}
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
      <button type="button" className="text-button" onClick={showRegisterForm}>
        Register new learner
      </button>
      {registerMessage && <p className="success-message">{registerMessage}</p>}
      {error && <p className="login-error">{error}</p>}
    </section>
  );
}

export default LoginPanel;
