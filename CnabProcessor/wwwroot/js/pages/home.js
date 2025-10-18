/**
 * CNAB Processor - Home Page JavaScript
 * Handles file upload and store management functionality
 */

class CnabHomePage {
    constructor() {
        this.uploadForm = document.getElementById('uploadForm');
        this.fileInput = document.getElementById('file');
        this.loading = document.getElementById('loading');
        this.result = document.getElementById('result');
        this.storesContainer = document.getElementById('storesContainer');
        this.statsContainer = document.getElementById('stats-container');
        this.loadStoresBtn = document.getElementById('loadStoresBtn');
        
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadStores(); // Load stores on page load
    }

    bindEvents() {
        if (this.uploadForm) {
            this.uploadForm.addEventListener('submit', (e) => this.handleFileUpload(e));
        }
        
        if (this.fileInput) {
            this.fileInput.addEventListener('change', (e) => this.handleFileSelection(e));
        }
        
        if (this.loadStoresBtn) {
            this.loadStoresBtn.addEventListener('click', () => this.loadStores());
        }
    }

    handleFileSelection(event) {
        const file = event.target.files[0];
        if (file) {
            this.updateFileDisplay(file);
        } else {
            this.resetFileDisplay();
        }
    }

    updateFileDisplay(file) {
        const fileUploadLabel = document.querySelector('.file-upload-label');
        if (fileUploadLabel) {
            fileUploadLabel.innerHTML = `
                <div>
                    <i class="fas fa-file-alt file-upload-icon"></i>
                    <div>
                        <strong>Selected: ${file.name}</strong>
                        <br>
                        <small>Size: ${this.formatFileSize(file.size)}</small>
                    </div>
                </div>
            `;
            fileUploadLabel.classList.add('file-selected');
        }
    }

    resetFileDisplay() {
        const fileUploadLabel = document.querySelector('.file-upload-label');
        if (fileUploadLabel) {
            fileUploadLabel.innerHTML = `
                <i class="fas fa-cloud-upload-alt file-upload-icon"></i>
                <div>
                    <strong>Click to select file</strong>
                    <br>
                    <small>Accepted formats: .txt</small>
                </div>
            `;
            fileUploadLabel.classList.remove('file-selected');
        }
    }

    resetUploadForm() {
        // Clear the file input
        if (this.fileInput) {
            this.fileInput.value = '';
        }
        
        // Reset the file display to default state
        this.resetFileDisplay();
        
        // Reset the form
        if (this.uploadForm) {
            this.uploadForm.reset();
        }
    }

    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }

    async handleFileUpload(event) {
        event.preventDefault();
        
        const file = this.fileInput.files[0];
        if (!file) {
            CnabUtils.showAlert('Please select a file to upload.', 'error');
            return;
        }

        if (!file.name.toLowerCase().endsWith('.txt')) {
            CnabUtils.showAlert('Please select a .txt file.', 'error');
            return;
        }

        CnabUtils.showLoading();
        
        try {
            const formData = new FormData();
            formData.append('file', file);
            
            const response = await fetch('/api/cnab/upload', {
                method: 'POST',
                body: formData
            });
            
            const result = await response.json();
            
            if (result.success) {
                CnabUtils.showAlert(result.message, 'success');
                this.loadStores(); // Refresh stores after upload
                this.resetUploadForm(); // Reset upload form after successful upload
            } else {
                CnabUtils.showAlert(result.message, 'error');
            }
        } catch (error) {
            console.error('Upload error:', error);
            CnabUtils.showAlert('An error occurred while uploading the file.', 'error');
        } finally {
            CnabUtils.hideLoading();
        }
    }

    async loadStores() {
        try {
            const stores = await CnabUtils.request('/api/stores');
            this.displayStores(stores);
            this.displayStats(stores);
        } catch (error) {
            console.error('Error loading stores:', error);
            CnabUtils.showAlert('Error loading stores data.', 'error');
        }
    }

    displayStores(stores) {
        if (!this.storesContainer) return;

        if (stores.length === 0) {
            this.storesContainer.innerHTML = `
                <div class="empty-state">
                    <i class="fas fa-inbox empty-state-icon"></i>
                    <h3>No stores processed</h3>
                    <p>Upload a CNAB file and click "Load Stores"</p>
                </div>
            `;
            return;
        }

        const storesHtml = `
            <table class="stores-table">
                <thead>
                    <tr>
                        <th>Store Name</th>
                        <th>Owner</th>
                        <th>Balance</th>
                        <th>Transactions</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${stores.map(store => `
                        <tr class="store-row" data-store-id="${store.id}">
                            <td class="store-name">${CnabUtils.escapeHtml(store.name)}</td>
                            <td class="store-owner">${CnabUtils.escapeHtml(store.owner)}</td>
                            <td class="store-balance ${store.balance < 0 ? 'negative' : 'positive'}">${CnabUtils.formatCurrency(store.balance)}</td>
                            <td class="store-transactions">${store.transactionCount}</td>
                            <td class="store-actions">
                                <button class="btn-toggle-transactions" data-store-id="${store.id}">
                                    <i class="fas fa-chevron-down"></i>
                                    View Transactions
                                </button>
                            </td>
                        </tr>
                        <tr class="transactions-row" data-store-id="${store.id}" style="display: none;">
                            <td colspan="5" class="transactions-container">
                                <div class="transactions-loading">
                                    <div class="spinner"></div>
                                    <span>Loading transactions...</span>
                                </div>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;

        this.storesContainer.innerHTML = storesHtml;
        this.bindTransactionEvents();
    }

    bindTransactionEvents() {
        const toggleButtons = document.querySelectorAll('.btn-toggle-transactions');
        toggleButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const storeId = e.target.getAttribute('data-store-id');
                this.toggleTransactions(storeId);
            });
        });
    }

    async toggleTransactions(storeId) {
        const transactionsRow = document.querySelector(`.transactions-row[data-store-id="${storeId}"]`);
        const toggleButton = document.querySelector(`.btn-toggle-transactions[data-store-id="${storeId}"]`);
        const icon = toggleButton.querySelector('i');
        
        if (transactionsRow.style.display === 'none') {
            // Show transactions
            transactionsRow.style.display = 'table-row';
            icon.className = 'fas fa-chevron-up';
            toggleButton.innerHTML = '<i class="fas fa-chevron-up"></i> Hide Transactions';
            
            // Load transactions if not already loaded
            const container = transactionsRow.querySelector('.transactions-container');
            if (container.querySelector('.transactions-loading')) {
                await this.loadStoreTransactions(storeId, container);
            }
        } else {
            // Hide transactions
            transactionsRow.style.display = 'none';
            icon.className = 'fas fa-chevron-down';
            toggleButton.innerHTML = '<i class="fas fa-chevron-down"></i> View Transactions';
        }
    }

    async loadStoreTransactions(storeId, container) {
        try {
            const response = await fetch(`/api/stores/${storeId}/transactions`);
            const transactions = await response.json();
            
            if (transactions.length === 0) {
                container.innerHTML = '<div class="no-transactions">No transactions found for this store.</div>';
                return;
            }

            const transactionsHtml = `
                <div class="transactions-list">
                    <h4>Store Transactions</h4>
                    <table class="transactions-table">
                        <thead>
                            <tr>
                                <th>Date</th>
                                <th>Type</th>
                                <th>Nature</th>
                                <th>Amount</th>
                                <th>CPF</th>
                                <th>Card</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${transactions.map(transaction => {
                                const effectiveAmount = this.getEffectiveAmount(transaction.type, transaction.amount);
                                const amountClass = effectiveAmount < 0 ? 'negative' : 'positive';
                                const typeDescription = this.getTransactionTypeDescription(transaction.type);
                                
                                const nature = this.getNature(transaction.type);
                                
                                return `
                                    <tr>
                                        <td class="transaction-date">${this.formatDate(transaction.date)}</td>
                                        <td class="transaction-type" title="Type ${transaction.type}">
                                            <span class="type-number">${CnabUtils.escapeHtml(transaction.type)}</span>
                                            <span class="type-description">${typeDescription}</span>
                                        </td>
                                        <td class="transaction-nature">${nature}</td>
                                        <td class="transaction-amount ${amountClass}">${CnabUtils.formatCurrency(effectiveAmount)}</td>
                                        <td class="transaction-cpf">${CnabUtils.escapeHtml(transaction.cpf)}</td>
                                        <td class="transaction-card">${CnabUtils.escapeHtml(transaction.card)}</td>
                                    </tr>
                                `;
                            }).join('')}
                        </tbody>
                    </table>
                </div>
            `;
            
            container.innerHTML = transactionsHtml;
        } catch (error) {
            console.error('Error loading transactions:', error);
            container.innerHTML = '<div class="error-message">Error loading transactions. Please try again.</div>';
        }
    }

    formatDate(dateString) {
        try {
            // Parse the UTC date string from the database
            const utcDate = new Date(dateString);
            
            // Check if the date is valid
            if (isNaN(utcDate.getTime())) {
                return 'Invalid Date';
            }
            
            const day = utcDate.getDate().toString().padStart(2, '0');
            const month = (utcDate.getMonth() + 1).toString().padStart(2, '0');
            const year = utcDate.getFullYear();
            const hours = utcDate.getHours().toString().padStart(2, '0');
            const minutes = utcDate.getMinutes().toString().padStart(2, '0');
            
            return `${day}/${month}/${year} ${hours}:${minutes}`;
        } catch (error) {
            console.error('Error formatting date:', error);
            return 'Invalid Date';
        }
    }

    isDebitTransaction(type) {
        const debitTypes = [2, 3, 9];
        return debitTypes.includes(parseInt(type));
    }

    getEffectiveAmount(type, amount) {
        return this.isDebitTransaction(type) ? -Math.abs(amount) : Math.abs(amount);
    }

    getTransactionTypeDescription(type) {
        const descriptions = {
            1: 'Debit',
            2: 'Boleto',
            3: 'Financing',
            4: 'Credit',
            5: 'Loan Receipt',
            6: 'Sales',
            7: 'TED Receipt',
            8: 'DOC Receipt',
            9: 'Rent'
        };
        return descriptions[parseInt(type)] || `Type ${type}`;
    }

    getNature(type) {
        return this.isDebitTransaction(type) ? 'Out' : 'In';
    }

    getSign(type) {
        return this.isDebitTransaction(type) ? '-' : '+';
    }

    displayStats(stores) {
        if (!this.statsContainer) return;

        if (stores.length === 0) {
            this.statsContainer.style.display = 'none';
            return;
        }

        const totalStores = stores.length;
        const totalTransactions = stores.reduce((sum, store) => sum + store.transactionCount, 0);
        const totalBalance = stores.reduce((sum, store) => sum + store.balance, 0);
        const avgBalance = totalStores > 0 ? totalBalance / totalStores : 0;

        this.statsContainer.innerHTML = `
            <div class="stat-card">
                <div class="stat-value">${totalStores}</div>
                <div class="stat-label">Total Stores</div>
            </div>
            <div class="stat-card">
                <div class="stat-value">${totalTransactions}</div>
                <div class="stat-label">Total Transactions</div>
            </div>
            <div class="stat-card">
                <div class="stat-value ${totalBalance < 0 ? 'negative' : 'positive'}">${CnabUtils.formatCurrency(totalBalance)}</div>
                <div class="stat-label">Total Balance</div>
            </div>
            <div class="stat-card">
                <div class="stat-value ${avgBalance < 0 ? 'negative' : 'positive'}">${CnabUtils.formatCurrency(avgBalance)}</div>
                <div class="stat-label">Average Balance</div>
            </div>
        `;
        
        this.statsContainer.style.display = 'grid';
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new CnabHomePage();
});

function loadStores() {
    const homePage = new CnabHomePage();
    homePage.loadStores();
}
