# CNAB Processor - Frontend Assets

## 📁 File Structure

```
wwwroot/
├── css/
│   ├── site.css                 # Main application CSS
│   └── components/
│       ├── cards.css           # Card component styles
│       └── forms.css           # Form component styles
├── js/
│   ├── site.js                 # Global JavaScript utilities
│   └── pages/
│       └── home.js             # Home page specific JavaScript
└── README.md                   # This documentation
```

## 🎨 CSS Organization

### **site.css** - Main CSS
- Reset and base styles
- General layout
- Responsiveness
- Imports components

### **components/cards.css** - Card Components
- `.card` - Base card
- `.card-header` - Card header
- `.card-body` - Card body
- `.store-item` - Store item
- `.stat-card` - Statistics card
- `.empty-state` - Empty state

### **components/forms.css** - Form Components
- `.form-group` - Form group
- `.form-control` - Form controls
- `.file-upload` - File upload
- `.btn` - Buttons
- `.loading` - Loading states
- `.alert` - Alerts

## 🚀 JavaScript Organization

### **site.js** - Global JavaScript
- Global utilities (`CnabUtils`)
- Formatting functions
- Global error handling
- Global initialization

### **pages/home.js** - Home Page JavaScript
- `CnabHomePage` class
- File upload handling
- Store loading and display
- Transaction display with new column order (Date, Type, Nature, Amount, CPF, Card)
- Statistics display
- **Updated API endpoints**: Uses `/api/stores` instead of `/api/cnab/stores`
- Page-specific events

## 📋 Conventions

### **CSS**
- BEM classes when appropriate
- Modular components
- Mobile-first responsiveness
- CSS variables for colors and spacing

### **JavaScript**
- ES6 classes for organization
- Event delegation
- Async/await for requests
- Consistent error handling

### **Naming**
- Files in kebab-case
- Classes in PascalCase
- IDs in camelCase
- Variables in camelCase

## 🔧 How to Add New Pages

1. **Create specific CSS** (if needed):
   ```css
   /* components/new-page.css */
   .new-page-component {
       /* specific styles */
   }
   ```

2. **Create specific JavaScript**:
   ```javascript
   // pages/new-page.js
   class NewPage {
       constructor() {
           this.init();
       }
       
       init() {
           this.bindEvents();
       }
   }
   ```

3. **Import in site.css**:
   ```css
   @import url('components/new-page.css');
   ```

4. **Reference in view**:
   ```html
   @section Scripts {
       <script src="~/js/pages/new-page.js" asp-append-version="true"></script>
   }
   ```

## 🎯 Structure Benefits

- ✅ **Modularity**: Each component has its responsibility
- ✅ **Maintainability**: Easy to find and edit styles
- ✅ **Scalability**: Easy to add new pages
- ✅ **Performance**: Page-specific CSS and JS
- ✅ **Organization**: Clear and logical structure
- ✅ **Reusability**: Components can be reused
