const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5029').replace(/\/$/, '');
const AUTH_TOKEN_KEY = 'azureknowledgehub.jwt';

function getStoredToken() {
  return localStorage.getItem(AUTH_TOKEN_KEY);
}

function setStoredToken(token) {
  localStorage.setItem(AUTH_TOKEN_KEY, token);
}

function clearStoredToken() {
  localStorage.removeItem(AUTH_TOKEN_KEY);
}

async function request(path, options = {}) {
  const { params = {}, method = 'GET', body } = options;
  const url = new URL(`${API_BASE_URL}${path}`);

  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      url.searchParams.set(key, value);
    }
  });

  const headers = {
    Accept: 'application/json',
  };

  const token = getStoredToken();
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  if (body) {
    headers['Content-Type'] = 'application/json';
  }

  const response = await fetch(url, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    throw new Error(`API request failed with status ${response.status}`);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export function getLearningResources() {
  return request('/api/learningresources');
}

export function getLearningResourceById(id) {
  return request(`/api/learningresources/${id}`);
}

export function searchLearningResources(params) {
  return request('/api/search/resources', { params });
}

export function createLearningResource(resource) {
  return request('/api/learningresources', {
    method: 'POST',
    body: resource,
  });
}

export function updateLearningResource(id, resource) {
  return request(`/api/learningresources/${id}`, {
    method: 'PUT',
    body: resource,
  });
}

export function deleteLearningResource(id) {
  return request(`/api/learningresources/${id}`, {
    method: 'DELETE',
  });
}

export function loginUser(credentials) {
  return request('/api/auth/login', {
    method: 'POST',
    body: credentials,
  });
}

export function getProfile() {
  return request('/api/profile');
}

export { API_BASE_URL, clearStoredToken, getStoredToken, setStoredToken };
