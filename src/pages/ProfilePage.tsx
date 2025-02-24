import React, { useState, useEffect } from 'react';
import { api, type Profile } from '../services/api';
import './ProfilePage.css';

const ProfilePage: React.FC = () => {
  const [profile, setProfile] = useState<Profile | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [isUploading, setIsUploading] = useState(false);

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) throw new Error('No auth token');

      const data = await api.getProfile(token);
      setProfile(data);
    } catch (err) {
      setError('Failed to load profile');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      setError('Please upload an image file');
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      setError('Image size should be less than 5MB');
      return;
    }

    setIsUploading(true);
    try {
      const token = localStorage.getItem('token');
      if (!token) throw new Error('No auth token');

      const response = await api.uploadProfileImage(token, file);
      
      if (profile && response.imageUrl) {
        setProfile({
          ...profile,
          profileImageUrl: response.imageUrl
        });
      }
    } catch (err) {
      setError('Failed to upload image');
      console.error(err);
    } finally {
      setIsUploading(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    if (!profile) return;
    const { name, value } = e.target;
    setProfile({
      ...profile,
      [name]: value
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!profile) return;
    try {
      const token = localStorage.getItem('token');
      if (!token) throw new Error('No auth token');
      await api.updateProfile(token, profile);
      setIsEditing(false);
      fetchProfile();
    } catch (err) {
      setError('Failed to update profile');
    }
  };

  if (loading) return <div className="profile-loading">Loading...</div>;
  if (error) return <div className="profile-error">{error}</div>;
  if (!profile) return <div className="profile-error">No profile found</div>;

  return (
    <div className="profile-page">
      <div className="profile-container">
        <div className="profile-header">
          <div className="profile-image-container">
            <img 
              src={profile.profileImageUrl || '/default-avatar.png'} 
              alt="Profile" 
              className="profile-image" 
            />
            {isEditing && (
              <div className="image-upload-overlay">
                <input
                  type="file"
                  id="profile-image-upload"
                  accept="image/*"
                  onChange={handleImageUpload}
                  className="hidden"
                  disabled={isUploading}
                />
                <label 
                  htmlFor="profile-image-upload" 
                  className="change-photo-btn"
                >
                  {isUploading ? 'Uploading...' : 'Change Photo'}
                </label>
              </div>
            )}
          </div>
        </div>

        <form onSubmit={handleSubmit} className="profile-form">
          <div className="form-group">
            <label>First Name</label>
            <input
              type="text"
              name="firstName"
              value={profile.firstName}
              onChange={handleInputChange}
              disabled={!isEditing}
              className="form-input"
            />
          </div>
          <div className="profile-actions">
            {isEditing ? (
              <>
                <button type="submit" className="save-btn">Save Changes</button>
                <button type="button" onClick={() => setIsEditing(false)} className="cancel-btn">Cancel</button>
              </>
            ) : (
              <button type="button" onClick={() => setIsEditing(true)} className="edit-btn">Edit Profile</button>
            )}
          </div>
        </form>
      </div>
    </div>
  );
};

export default ProfilePage;
