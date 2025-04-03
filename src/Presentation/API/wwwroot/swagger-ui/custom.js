// Add language selector to the Swagger UI
window.onload = function() {
    setTimeout(function() {
        // Create language selector container
        var languageSelector = document.createElement('div');
        languageSelector.className = 'language-selector';
        
        // Create label
        var label = document.createElement('label');
        label.innerHTML = 'Language:';
        languageSelector.appendChild(label);
        
        // Create select element
        var select = document.createElement('select');
        select.id = 'language-select';
        
        // Add language options
        var englishOption = document.createElement('option');
        englishOption.value = 'en';
        englishOption.text = 'English';
        
        var arabicOption = document.createElement('option');
        arabicOption.value = 'ar';
        arabicOption.text = 'العربية';
        
        select.appendChild(englishOption);
        select.appendChild(arabicOption);
        
        // Set event listener for language change
        select.addEventListener('change', function() {
            changeLanguage(this.value);
        });
        
        languageSelector.appendChild(select);
        
        // Append language selector to Swagger UI
        var topbarElement = document.querySelector('.swagger-ui .topbar');
        if (topbarElement) {
            topbarElement.appendChild(languageSelector);
        }
        
        // Set initial language based on browser preference
        var userLanguage = navigator.language || navigator.userLanguage;
        if (userLanguage && userLanguage.startsWith('ar')) {
            select.value = 'ar';
            changeLanguage('ar');
        }
    }, 500);
};

// Function to change the language
function changeLanguage(language) {
    var swaggerUI = document.querySelector('.swagger-ui');
    
    // Toggle RTL class for Arabic
    if (language === 'ar') {
        swaggerUI.classList.add('rtl');
        document.documentElement.setAttribute('lang', 'ar');
        document.documentElement.setAttribute('dir', 'rtl');
        translateToArabic();
    } else {
        swaggerUI.classList.remove('rtl');
        document.documentElement.setAttribute('lang', 'en');
        document.documentElement.setAttribute('dir', 'ltr');
        resetToEnglish();
    }
    
    // Store language preference
    localStorage.setItem('preferred_language', language);
    
    // Notify backend about language change (could be implemented with an API call)
    updateApiHeader(language);
}

// Function to update API headers with language preference
function updateApiHeader(language) {
    // This will be used to set the Accept-Language header for all API requests
    window.swaggerUiAuth = window.swaggerUiAuth || {};
    window.swaggerUiAuth.extraHeaders = window.swaggerUiAuth.extraHeaders || {};
    window.swaggerUiAuth.extraHeaders['Accept-Language'] = language;
    
    // Update existing authorization
    var authorizationHeader = localStorage.getItem('auth_token');
    if (authorizationHeader) {
        window.swaggerUiAuth.extraHeaders['Authorization'] = authorizationHeader;
    }
}

// Function to translate UI elements to Arabic
function translateToArabic() {
    // Simple example of translating some UI elements
    // In a real implementation, this would be more comprehensive
    var translations = {
        'Models': 'النماذج',
        'Schemas': 'المخططات',
        'Parameters': 'المعلمات',
        'Responses': 'الاستجابات',
        'Try it out': 'جرب الآن',
        'Execute': 'تنفيذ',
        'Authorize': 'تفويض',
        'Available authorizations': 'التفويضات المتاحة',
        'Cancel': 'إلغاء',
        'Clear': 'مسح',
        'Curl': 'أمر كيرل',
        'Request URL': 'عنوان URL للطلب',
        'Server response': 'استجابة الخادم',
        'Response body': 'محتوى الاستجابة',
        'Response headers': 'رؤوس الاستجابة',
        'Example Value': 'قيمة المثال',
        'Model': 'نموذج',
        'Description': 'وصف',
        'Authentication': 'المصادقة'
    };
    
    // Apply translations
    for (var key in translations) {
        var elements = document.querySelectorAll('.swagger-ui div, .swagger-ui button, .swagger-ui h4, .swagger-ui span');
        for (var i = 0; i < elements.length; i++) {
            if (elements[i].innerText === key) {
                elements[i].innerText = translations[key];
            }
        }
    }
}

// Function to reset UI elements to English
function resetToEnglish() {
    // This would reload the page to reset to English
    // In a production environment, a more sophisticated approach would be used
    location.reload();
} 