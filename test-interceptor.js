// Test if the interceptor is working
// Run this in browser console after the operator dashboard loads

console.log('🧪 TESTING INTERCEPTOR...');

// Inject HttpClient and test
const injector = window.ng?.getInjector?.(document.querySelector('app-root'));
if (injector) {
    const httpClient = injector.get('HttpClient');
    if (httpClient) {
        console.log('✅ HttpClient found, testing...');
        
        // Test the routes endpoint through Angular HttpClient
        httpClient.get('http://localhost:5266/api/operator-dashboard/routes').subscribe({
            next: (data) => console.log('✅ Angular HttpClient SUCCESS:', data),
            error: (error) => console.error('❌ Angular HttpClient ERROR:', error)
        });
    } else {
        console.error('❌ HttpClient not found');
    }
} else {
    console.error('❌ Angular injector not found');
    console.log('Try this alternative test:');
    console.log(`
// Alternative test - make a request through the service
const component = document.querySelector('app-operator-dashboard');
if (component) {
    // Access the component instance if possible
    console.log('Component found:', component);
} else {
    console.log('Component not found');
}
`);
}