// JavaScript functions for auth state management
// These functions can be called from Blazor to read/write auth state

window.authStateInterop = {
    // Read auth data from localStorage
    getAuthData: function() {
        try {
            const authData = localStorage.getItem('authData');
            return authData || null;
        } catch (e) {
            console.error('Error reading authData from localStorage:', e);
            return null;
        }
    },
    
    // Check if auth data exists
    hasAuthData: function() {
        try {
            const authData = localStorage.getItem('authData');
            return authData !== null && authData.length > 0;
        } catch (e) {
            console.error('Error checking authData in localStorage:', e);
            return false;
        }
    },
    
    // Set up a callback to notify when auth state changes
    // This uses a polling mechanism since we can't use events directly
    watchAuthState: function(dotNetHelper, intervalMs) {
        let lastAuthData = localStorage.getItem('authData');
        
        const checkInterval = setInterval(function() {
            try {
                const currentAuthData = localStorage.getItem('authData');
                if (currentAuthData !== lastAuthData) {
                    lastAuthData = currentAuthData;
                    if (dotNetHelper && dotNetHelper.invokeMethodAsync) {
                        dotNetHelper.invokeMethodAsync('OnAuthDataChanged', currentAuthData);
                    }
                }
            } catch (e) {
                console.error('Error watching auth state:', e);
            }
        }, intervalMs || 500);
        
        // Return cleanup function
        return function() {
            clearInterval(checkInterval);
        };
    }
};

