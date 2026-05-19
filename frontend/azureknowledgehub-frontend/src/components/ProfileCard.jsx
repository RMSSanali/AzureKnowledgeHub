function ProfileCard({ user }) {
  if (!user) {
    return null;
  }

  return (
    <section className="profile-card" aria-label="Profile">
      <p className="eyebrow">Profile</p>
      <h2>{user.displayName || user.username}</h2>
      <dl className="profile-list">
        <div>
          <dt>Username</dt>
          <dd>{user.username}</dd>
        </div>
        <div>
          <dt>Email</dt>
          <dd>{user.email}</dd>
        </div>
        <div>
          <dt>Role</dt>
          <dd>{user.role}</dd>
        </div>
      </dl>
    </section>
  );
}

export default ProfileCard;
