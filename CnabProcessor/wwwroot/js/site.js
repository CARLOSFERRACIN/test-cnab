/**
 * CNAB Processor - Global JavaScript
 * Contains global utilities and common functionality
 */

// Global utility functions
window.CnabUtils = {
    /**
     * Show loading state
     */
    showLoading: function(elementId = 'loading') {
        const element = document.getElementById(elementId);
        if (element) {
            element.classList.add('show');
        }
    },

    /**
     * Hide loading state
     */
    hideLoading: function(elementId = 'loading') {
        const element = document.getElementById(elementId);
        if (element) {
            element.classList.remove('show');
        }
    },

    /**
     * Show alert message
     */
    showAlert: function(message, type = 'info', containerId = 'result') {
        const container = document.getElementById(containerId);
        if (!container) return;

        const alertClass = type === 'error' ? 'alert-error' : 
                          type === 'success' ? 'alert-success' : 'alert-info';
        
        container.innerHTML = `
            <div class="alert ${alertClass}">
                ${this.escapeHtml(message)}
            </div>
        `;
        
        // Auto-hide success messages after 5 seconds
        if (type === 'success') {
            setTimeout(() => {
                container.innerHTML = '';
            }, 5000);
        }
    },

    /**
     * Escape HTML to prevent XSS
     */
    escapeHtml: function(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    },

    /**
     * Format currency for Brazilian Real
     */
    formatCurrency: function(value) {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    },

    /**
     * Make HTTP request with error handling
     */
    request: async function(url, options = {}) {
        try {
            const response = await fetch(url, {
                headers: {
                    'Content-Type': 'application/json',
                    ...options.headers
                },
                ...options
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error('Request error:', error);
            throw error;
        }
    }
};

// Global error handler
window.addEventListener('error', function(event) {
    console.error('Global error:', event.error);
});

// Global unhandled promise rejection handler
window.addEventListener('unhandledrejection', function(event) {
    console.error('Unhandled promise rejection:', event.reason);
});

// Initialize global functionality when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Add any global initialization here
    console.log('CNAB Processor - Global scripts loaded');
});