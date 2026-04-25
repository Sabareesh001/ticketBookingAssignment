// Run this in your browser console (F12 -> Console tab)
// This will check the status of your operator token

console.log('=== OPERATOR TOKEN STATUS CHECK ===');

const operatorToken = localStorage.getItem('operator_token');
const operatorUser = localStorage.getItem('operator_user');

if (!operatorToken) {
    console.error('❌ NO OPERATOR TOKEN FOUND');
    console.log('You need to log in at /operator-login to get a new token');
} else {
    console.log('✅ Operator token exists');
    
    try {
        const parts = operatorToken.split('.');
        if (parts.length !== 3) {
            console.error('❌ Invalid token format');
        } else {
            const payload = JSON.parse(atob(parts[1]));
            const exp = new Date(payload.exp * 1000);
            const now = new Date();
            const isExpired = now >= exp;
            
            console.log('Token Details:');
            console.log('- Operator ID:', payload.nameid || payload.sub);
            console.log('- Operator Name:', payload.unique_name || payload.name);
            console.log('- Expires at:', exp.toLocaleString());
            console.log('- Current time:', now.toLocaleString());
            console.log('- Status:', isExpired ? '❌ EXPIRED' : '✅ VALID');
            
            if (isExpired) {
                console.error('Token is expired! You need to log in again.');
                console.log('Run this to clear the expired token:');
                console.log('localStorage.removeItem("operator_token"); localStorage.removeItem("operator_user"); location.reload();');
            }
        }
    } catch (e) {
        console.error('❌ Error parsing token:', e);
    }
}

if (operatorUser) {
    console.log('✅ Operator user data exists');
    try {
        const user = JSON.parse(operatorUser);
        console.log('User:', user);
    } catch (e) {
        console.error('Error parsing user data:', e);
    }
} else {
    console.log('❌ No operator user data found');
}

console.log('=== END CHECK ===');
