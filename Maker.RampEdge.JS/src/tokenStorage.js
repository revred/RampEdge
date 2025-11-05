/**
 * TokenStorage class handles the storage and retrieval of authentication tokens.
 * Currently implements in-memory storage, but can be extended to use localStorage,
 * sessionStorage, or other storage mechanisms.
 */
class TokenStorage {
    constructor() {
        this.storage = {
            accessToken: null,
            refreshToken: null
        };
    }

    /**
     * Store both access and refresh tokens
     * @param {string} accessToken - The access token to store
     * @param {string} refreshToken - The refresh token to store
     */
    setTokens(accessToken, refreshToken) {
        this.storage.accessToken = accessToken;
        this.storage.refreshToken = refreshToken;
    }

    /**
     * Get the stored access token
     * @returns {string|null} The stored access token or null if not set
     */
    getAccessToken() {
        return this.storage.accessToken;
    }

    /**
     * Get the stored refresh token
     * @returns {string|null} The stored refresh token or null if not set
     */
    getRefreshToken() {
        return this.storage.refreshToken;
    }

    /**
     * Check if the user is currently authorized (has an access token)
     * @returns {boolean} True if an access token is present
     */
    isAuthorized() {
        return !!this.storage.accessToken;
    }

    /**
     * Clear all stored tokens
     */
    clearTokens() {
        this.storage.accessToken = null;
        this.storage.refreshToken = null;
    }
}

// Export a singleton instance to be used across the application
export const tokenStorage = new TokenStorage();

// Also export the class in case a new instance is needed
export { TokenStorage };