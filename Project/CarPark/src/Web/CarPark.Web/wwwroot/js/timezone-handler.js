// Minimal timezone handler - only sets client offset for server processing
(function() {
    'use strict';

    // Set client timezone offset in cookie for server-side processing
    function setClientTimezoneOffset() {
        const offset = new Date().getTimezoneOffset();
        
        // Check if cookie already exists with same value
        const existingCookie = document.cookie
            .split('; ')
            .find(row => row.startsWith('client-timezone-offset='));
        
        if (!existingCookie || existingCookie.split('=')[1] !== offset.toString()) {
            document.cookie = `client-timezone-offset=${offset}; path=/; SameSite=Strict; max-age=2592000`;
        }
    }

    // Initialize immediately
    setClientTimezoneOffset();
    
    // Update on timezone changes (rare but possible)
    let lastOffset = new Date().getTimezoneOffset();
    setInterval(() => {
        const currentOffset = new Date().getTimezoneOffset();
        if (currentOffset !== lastOffset) {
            lastOffset = currentOffset;
            setClientTimezoneOffset();
        }
    }, 60000); // Check every minute
})();
