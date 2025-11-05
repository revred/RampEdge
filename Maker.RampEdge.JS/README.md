
## Maker.RampEdge

Maker.RampEdge is a comprehensive client library for interacting with the Maker Platform API. It provides a seamless interface for managing product groups, user authentication, shopping carts, and e-commerce operations. The library handles all the complexity of API communication, token management, and error handling, allowing developers to focus on building their applications.

## Features

- **Robust Authentication**
  - Token-based authentication system
  - Automatic token refresh handling
  - Secure token storage management
  - SSO integration support

- **E-commerce Integration**
  - Shopping cart management
  - Product catalog access
  - Order processing
  - Stripe payment integration
  - Address management

- **Developer Experience**
  - Automatic error handling
  - Type-safe API interfaces
  - Comprehensive documentation
  - Easy-to-use API methods
  - Built-in retry mechanisms
  - Request/Response interceptors

- **Performance & Security**
  - Efficient request caching
  - Automatic token renewal
  - Secure credential handling
  - Rate limiting protection
  - Connection pooling

## Available methods (summary)

### Authentication Methods
- `registerUser(email, password)` - Register a new user
- `login(username, password)` - Login and get authentication tokens
- `refreshToken()` - Refresh an expired access token
- `requestSsoCode()` - Request SSO authentication code

### Authorized Methods (Requires Authentication)
- `getAllOrders(page, pageSize)` - Get user's order history
- `getUser(userDetail)` - Get user profile information
- `getCart()` - Get user's shopping cart
- `clearCart()` - Clear user's shopping cart
- `getAddress()` - Get user's saved addresses
- `upsertAddress(request)` - Create or update user's address
- `addProductsToCart(cartItems)` - Add items to cart
- `removeProductFromCart(slug)` - Remove item from cart
- `createCheckoutSession(request)` - Create Stripe checkout session
- `addProductReport(request)` - Submit a product report
- `updateProductReport(request)` - Update an existing report
- `addRating(request)` - Add product rating
- `insertUserActivity(request)` - Track user activity
- `sendStripeWebhook(payloadJson, signature)` - Process Stripe webhook (testing helper)

### Public Methods (No Authentication Required)
- `getProductGroups()` - Get all product groups
- `getProductsBySlug(slug)` - Get product by slug
- `getProductDetails(request)` - Get detailed product information
- `getReportByProduct(barID)` - Get product reports
- `getRatingByProduct({ barID })` - Get product ratings
- `getAboutInfo()` - Get about page information
- `getBlogs()` - Get blog posts
- `getDocumentation()` - Get documentation
- `getServices()` - Get available services


## Installation

npm install

Install the main package:
```bash
npm install maker-ramp-edge
```
```bash
npm install dotenv
```
Required peer dependencies:
```bash
npm install axios
```
## Configure Environment Variables

Create a .env file at the root of your project:

```
BUSINESS_UNIT_KEY=your_business_unit_key
API_BASE_URL=https://your.api.base.url
```

Note: All methods return a promise which resolves to the API response (or throws an error via the client's error handler). For authorized methods, the client automatically handles token management - you don't need to pass tokens manually.

## Add this to the top of your index.js to load .env variables:

```javascript
import dotenv from 'dotenv';
dotenv.config();
```

### Basic Usage

## Initializing the Client

The ProductGroupClient requires two parameters: businessUnitKey and apiBaseUrl.
```javascript
import { ProductGroupClient } from 'maker-ramp-edge';

const client = new ProductGroupClient(
  "YOUR_BUSINESS_UNIT_KEY",
  "https://your-api-base-url.com"
);
```

### Authentication and Protected Endpoints Example

```javascript
import { ProductGroupClient } from 'maker-ramp-edge';

async function example() {
  // Initialize the client
   const client = new ProductGroupClient(
    'your-business-unit-key',
    'https://your-api-base-url.com'
  );

  try {
    // Login to get authentication tokens
    await client.login('your-email@example.com', 'your-password');
    
    // After successful login, tokens are automatically stored
    // Now you can call any protected endpoint
    
    // Example: Get user's cart
    const cart = await client.getCart();
    console.log('Cart contents:', cart);
    
    // The client will automatically:
    // - Use the stored token for authentication
    // - Refresh the token if it expires
    // - Handle unauthorized errors
  } catch (error) {
    console.error('Error:', error.message);
  }
}
```

### Example in React Component
```javascript
import { ProductGroupClient } from 'maker-ramp-edge';

// Create a new instance with your business unit key
const client = new ProductGroupClient(
    'your-business-unit-key',
    'https://your-api-base-url.com'
  );

// Example usage in a React component
function ProductList() {
  const [products, setProducts] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchProducts() {
      try {
        const data = await client.getProductGroups();
        setProducts(data.products);
      } catch (err) {
        setError(err.message);
      }
    }

    fetchProducts();
  }, []);

  if (error) return <div>Error: {error}</div>;
  if (!products) return <div>Loading...</div>;

  return (
    <div>
      {products.map(product => (
        <div key={product.slug}>
          <h2>{product.name}</h2>
          <p>ID: {product.friendlyID}</p>
          {product.asset && <img src={product.asset.url} alt={product.name} />}
        </div>
      ))}
    </div>
  );
}
```

### Project Structure

#### Node.js Client (`maker-ramp-edge`)
```
Maker.RampEdge/
├── src/
│   ├── index.js          # Main client implementation
│   └── tokenStorage.js   # Token management implementation
├── bin/
│   └── print-products.js # CLI utilities
├── dist/                 # Compiled output
│   ├── index.js
│   └── index.js.map
├── package.json         # Project configuration
├── tsconfig.json       # TypeScript configuration
├── .babelrc           # Babel configuration
└── .env              # Environment variables
```

