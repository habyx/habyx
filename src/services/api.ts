// src/services/api.ts

interface LoginCredentials {
    email: string;
    password: string;
}

interface RegisterData {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
}

export interface Profile {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    bio?: string;
    occupation?: string;
    location?: string;
    profileImageUrl?: string;
    skills?: string;
    education?: string;
}

const BASE_URL = 'https://habyx-gzdabgd7hxdwbbgf.centralus-01.azurewebsites.net/api';

export const api = {
    // Auth endpoints
    login: async (credentials: LoginCredentials) => {
        try {
            const response = await fetch(`${BASE_URL}/Auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(credentials)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Login failed');
            }

            const data = await response.json();
            if (!data.token) {
                throw new Error('Invalid response format');
            }

            return data;
        } catch (error) {
            console.error('Login error:', error);
            throw error;
        }
    },

    register: async (userData: RegisterData) => {
        try {
            const response = await fetch(`${BASE_URL}/Auth/register`, {
                method: 'POST',
                headers: { 
                    'Content-Type': 'application/json',
                    // 'Access-Control-Allow-Origin': '*'  // This line is usually not needed in client requests
                },
                body: JSON.stringify(userData)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Registration failed');
            }

            return response.json();
        } catch (error) {
            console.error('Registration error:', error);
            throw error;
        }
    },

    // Profile endpoints
    getProfile: async (token: string) => {
        try {
            const response = await fetch(`${BASE_URL}/UserProfiles`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to fetch profile');
            }

            return response.json();
        } catch (error) {
            console.error('Get profile error:', error);
            throw error;
        }
    },

    updateProfile: async (token: string, profileData: Partial<Profile>) => {
        try {
            const response = await fetch(`${BASE_URL}/UserProfiles`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(profileData)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to update profile');
            }

            return response.json();
        } catch (error) {
            console.error('Update profile error:', error);
            throw error;
        }
    },

    // Image upload endpoint
    uploadProfileImage: async (token: string, file: File) => {
        try {
            const formData = new FormData();
            formData.append('file', file);

            const response = await fetch(`${BASE_URL}/Friends/upload-profile-image`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                },
                body: formData
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to upload image');
            }

            return response.json();
        } catch (error) {
            console.error('Image upload error:', error);
            throw error;
        }
    },

    // Delete profile image endpoint
    deleteProfileImage: async (token: string) => {
        try {
            const response = await fetch(`${BASE_URL}/Friends/profile-image`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to delete image');
            }

            return response.json();
        } catch (error) {
            console.error('Delete image error:', error);
            throw error;
        }
    }
};

export type { LoginCredentials, RegisterData };