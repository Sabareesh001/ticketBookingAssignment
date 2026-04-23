# Login Page UI Guide

## Visual Layout

### Passenger Login (Default View)

```
╔═══════════════════════════════════════╗
║                                       ║
║              Login                    ║
║                                       ║
║  ┌─────────────────────────────────┐  ║
║  │ Email                           │  ║
║  │ [____________________________]   │  ║
║  └─────────────────────────────────┘  ║
║                                       ║
║  ┌─────────────────────────────────┐  ║
║  │ Password                        │  ║
║  │ [____________________________]   │  ║
║  └─────────────────────────────────┘  ║
║                                       ║
║  ┌─────────────────────────────────┐  ║
║  │        [Login Button]           │  ║
║  └─────────────────────────────────┘  ║
║                                       ║
║  Don't have an account? Sign up       ║
║  Forgot password?                     ║
║                                       ║
║  ─────────────────────────────────    ║
║  Login as Bus Operator →              ║
║                                       ║
╚═══════════════════════════════════════╝
```

### Operator Login (After Toggle)

```
╔═══════════════════════════════════════╗
║                                       ║
║        Bus Operator Login             ║
║                                       ║
║  ┌─────────────────────────────────┐  ║
║  │ Email                           │  ║
║  │ [____________________________]   │  ║
║  └─────────────────────────────────┘  ║
║                                       ║
║  ┌─────────────────────────────────┐  ║
║  │ Password                        │  ║
║  │ [____________________________]   │  ║
║  └─────────────────────────────────┘  ║
║                                       ║
║  ┌─────────────────────────────────┐  ║
║  │        [Login Button]           │  ║
║  └─────────────────────────────────┘  ║
║                                       ║
║  Don't have an account?               ║
║  Register as operator                 ║
║                                       ║
║  ─────────────────────────────────    ║
║  ← Back to Passenger Login            ║
║                                       ║
╚═══════════════════════════════════════╝
```

## Color Scheme

- **Background**: Gradient (Purple #667eea to #764ba2)
- **Card**: White
- **Text**: Dark Gray (#333)
- **Labels**: Medium Gray (#555)
- **Links**: Purple (#667eea)
- **Button**: Purple (#667eea)
- **Button Hover**: Darker Purple (#5568d3)
- **Error**: Red (#dc3545)
- **Error Background**: Light Red (#f8d7da)

## Interactive Elements

### Toggle Button
- **Text**: "Login as Bus Operator →" or "← Back to Passenger Login"
- **Style**: Link button (no background)
- **Color**: Purple (#667eea)
- **Hover**: Underline + Darker Purple
- **Position**: Bottom of card, separated by divider line

### Login Button
- **Text**: "Login" or "Logging in..."
- **Style**: Solid button
- **Color**: Purple (#667eea)
- **Hover**: Darker Purple + Slight lift effect
- **Disabled**: Gray with reduced opacity
- **Width**: Full width of form

### Links
- **Passenger**: "Sign up", "Forgot password?"
- **Operator**: "Register as operator"
- **Style**: Text links
- **Color**: Purple (#667eea)
- **Hover**: Underline

## Form Validation

### Email Field
- **Required**: Yes
- **Format**: Valid email
- **Error Message**: "Email is required" or "Email must be valid"
- **Invalid State**: Red border

### Password Field
- **Required**: Yes
- **Minimum Length**: 6 characters
- **Error Message**: "Password is required" or "Password must be at least 6 characters"
- **Invalid State**: Red border

## Responsive Design

### Desktop (> 768px)
- Card width: 400px
- Padding: 40px
- Font size: 16px (buttons), 14px (labels)

### Tablet (768px)
- Card width: 90% of screen
- Padding: 30px
- Font size: 16px (buttons), 14px (labels)

### Mobile (< 768px)
- Card width: 100% of screen
- Padding: 20px
- Font size: 14px (buttons), 12px (labels)

## Animation & Transitions

### Form Toggle
- **Duration**: Instant (no animation)
- **Effect**: Form switches immediately
- **Error Clear**: Errors clear when toggling

### Button Hover
- **Duration**: 0.3s
- **Effect**: Color change + slight lift
- **Easing**: Ease-in-out

### Input Focus
- **Duration**: 0.3s
- **Effect**: Border color change + shadow
- **Easing**: Ease-in-out

## Accessibility

- **Labels**: Associated with inputs via `for` attribute
- **Error Messages**: Displayed below invalid fields
- **Button States**: Disabled state clearly visible
- **Contrast**: All text meets WCAG AA standards
- **Focus**: Clear focus indicators on all interactive elements

## User Interactions

### Passenger Login Flow
1. User sees login form
2. Enters email and password
3. Clicks "Login" button
4. Form validates
5. If valid: Submits to API
6. If invalid: Shows error messages
7. On success: Redirects to `/dashboard`
8. On error: Shows error message

### Toggle to Operator Login
1. User clicks "Login as Bus Operator →"
2. Form clears
3. Title changes to "Bus Operator Login"
4. Links update to show operator signup
5. Toggle button changes to "← Back to Passenger Login"

### Operator Login Flow
1. User sees operator login form
2. Enters email and password
3. Clicks "Login" button
4. Form validates
5. If valid: Submits to API
6. If invalid: Shows error messages
7. On success: Redirects to `/operator-dashboard`
8. On error: Shows error message

## Error Handling

### Network Error
- Message: "Login failed"
- Display: Red alert box at top of form

### Invalid Credentials
- Message: "Invalid email or password"
- Display: Red alert box at top of form

### Validation Error
- Message: Field-specific error
- Display: Red text below field
- Border: Red border on field

### Server Error
- Message: "An error occurred during login"
- Display: Red alert box at top of form

## Loading State

### During Login
- Button text: "Logging in..."
- Button disabled: Yes
- Button opacity: 0.6
- Cursor: Not-allowed

## Success State

### After Login
- Redirect to appropriate dashboard
- Token stored in localStorage
- User/Operator info stored in localStorage
- Session established

## Notes

- Both forms use identical styling
- Toggle is smooth and instant
- Form validation happens on submit
- Errors are field-specific
- Loading state prevents double submission
- All interactive elements have hover states
- Mobile-friendly and responsive
