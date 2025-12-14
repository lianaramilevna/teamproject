// Utility function to clear all localStorage data
// Can be called from browser console: clearAllStorage()
window.clearAllStorage = function() {
    localStorage.clear();
    console.log('All localStorage data cleared');
    location.reload();
};

// Clear specific auth data
window.clearAuthData = function() {
    localStorage.removeItem('authData');
    console.log('Auth data cleared');
    location.reload();
};

