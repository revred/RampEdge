import axios from 'axios';
import dotenv from 'dotenv';
import { tokenStorage } from './tokenStorage.js';

dotenv.config(); // Load .env values

export class ProductGroupClient {
    constructor(
        businessUnitKey = null,
        apiBaseUrl = null
    ) {
        // Try user-provided values first, then environment variables
        this.businessUnitKey = businessUnitKey || process.env.BUSINESS_UNIT_KEY;
        this.API_BASE_URL = apiBaseUrl || process.env.API_BASE_URL;

        // Validate required configuration
        if (!this.businessUnitKey) {
            throw new Error('BusinessUnitKey is required. Provide it in constructor or set BUSINESS_UNIT_KEY environment variable.');
        }
        if (!this.API_BASE_URL) {
            throw new Error('ApiBaseUrl is required. Provide it in constructor or set API_BASE_URL environment variable.');
        }

        this.tokenStorage = tokenStorage;
    }

// ✅ Register New User
async registerUser(email, password) {
    try {
        const response = await axios.post(
            `${this.API_BASE_URL}/api/public/authentication/Register`,
            { email, password },
            {
                headers: {
                    'businessunitkey': this.businessUnitKey,
                    'Content-Type': 'application/json'
                }
            }
        );

        console.log(`🧾 User registered successfully: ${email}`);
        return response.data;
    } catch (error) {
        this.handleError(error);
    }
}

    // ✅ LOGIN
    async login(username, password) {
        try {
            // Combine email and password with '|'
            const emailAndPassword = `${username}|${password}`;

            const response = await axios.post(
                `${this.API_BASE_URL}/api/public/Login`,
                { emailAndPassword }, // body parameter
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Content-Type': 'application/json'
                    }
                }
            );

            // Store tokens from the response
            if (response.data?.token) {
                this.tokenStorage.setTokens(response.data.token, response.data.refreshToken);
                console.log('✅ Login successful and tokens stored');
            } else {
                throw new Error('No token received from server');
            }
            
            return response.data;
        } catch (error) {
            this.tokenStorage.clearTokens(); // Clear any existing tokens on login failure
            this.handleError(error);
        }
    }

    // ✅ Refresh Token
async refreshToken() {
    try {
        const refreshTokenValue = this.tokenStorage.getRefreshToken();
        if (!refreshTokenValue) {
            throw new Error('No refresh token available. Please login again.');
        }

        const response = await axios.post(
            `${this.API_BASE_URL}/api/public/authentication/refresh-token`,
            { refreshToken: refreshTokenValue },
            {
                headers: {
                    'businessunitkey': this.businessUnitKey,
                    'Content-Type': 'application/json'
                }
            }
        );

        // Store the new tokens
        if (response.data?.token) {
            this.tokenStorage.setTokens(response.data.token, response.data.refreshToken);
            console.log('🔄 Token refreshed successfully');
        } else {
            throw new Error('No token received from server');
        }

        return response.data;
    } catch (error) {
        this.tokenStorage.clearTokens(); // Clear tokens on refresh failure
        this.handleError(error);
    }
}

    // ✅ Cancel Product Order
    async cancelOrder(barId) {
        try {
            if (!this.tokenStorage.isAuthorized()) {
                throw new Error('Unauthorized: Please login first');
            }

            const token = this.tokenStorage.getAccessToken();
            const response = await axios.post(
                `${this.API_BASE_URL}/api/product/OrderCancel`,
                { barId }, // request body
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`🛑 Order with BarId ${barId} cancelled successfully.`);
            return response.data;
        } catch (error) {
            if (error.response?.status === 401) {
                this.tokenStorage.clearTokens(); // Clear tokens on unauthorized response
                throw new Error('Unauthorized: Your session has expired. Please login again.');
            }
            this.handleError(error);
        }
    }

    // ✅ Request SSO Code
    /**
     * Exchanges refresh/access tokens for a short-lived SSO code (valid ~3 minutes)
     * @param {Object} request - { refreshToken, accessToken }
     * @returns {Promise<any>} { code: string } or API error
     */
    async requestSsoCode() {
        try {
            if (!this.tokenStorage.isAuthorized()) {
                throw new Error('Unauthorized: Please login first');
            }

            const body = {
                refreshToken: this.tokenStorage.getRefreshToken() || '',
                accessToken: this.tokenStorage.getAccessToken() || ''
            };

            const response = await axios.post(
                `${this.API_BASE_URL}/api/public/authentication/RequestSsoCode`,
                body,
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`🔐 Requested SSO code`);
            return response.data;
        } catch (error) {
            if (error.response?.status === 401) {
                this.tokenStorage.clearTokens();
                throw new Error('Unauthorized: Your session has expired. Please login again.');
            }
            this.handleError(error);
        }
    }





    // ✅ Get All Orders
    async getAllOrders(page = 0, pageSize = 10) {
        try {
            if (!this.tokenStorage.isAuthorized()) {
                throw new Error('Unauthorized: Please login first');
            }

            const token = this.tokenStorage.getAccessToken();
            const response = await axios.post(
                `${this.API_BASE_URL}/api/product/GetAllOrders`,
                { page, pageSize },
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log('📦 Orders fetched successfully');
            return response.data;
        } catch (error) {
            if (error.response?.status === 401) {
                this.tokenStorage.clearTokens();
                throw new Error('Unauthorized: Your session has expired. Please login again.');
            }
            this.handleError(error);
        }
    }

    // ✅ Get all product groups
    async getProductGroups() {
        try {
            const response = await axios.get(
                `${this.API_BASE_URL}/api/public/ProductGroups`,
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey
                    }
                }
            );
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Get products by slug
    async getProductsBySlug(slug) {
        try {
            const response = await axios.post(
                `${this.API_BASE_URL}/api/public/ProductsBySlug`,
                { slug },
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Content-Type': 'application/json'
                    }
                }
            );
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Get User Details
    async getUser(userDetail) {
        try {
            if (!this.tokenStorage.isAuthorized()) {
                throw new Error('Unauthorized: Please login first');
            }

            const token = this.tokenStorage.getAccessToken();
            const response = await axios.post(
                `${this.API_BASE_URL}/api/entity/User`,
                { userDetail },
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`👤 User details fetched successfully for: ${userDetail}`);
            return response.data;
        } catch (error) {
            if (error.response?.status === 401) {
                this.tokenStorage.clearTokens();
                throw new Error('Unauthorized: Your session has expired. Please login again.');
            }
            this.handleError(error);
        }
    }

    // ✅ Add Product Report
    /**
     * Adds a new product report.
     * A ccepts a request object matching the C# AddProductReportRequest:
     * {
     *   productBarID: number,
     *   message: string,
     *   reportType: string | null,
     *   readMe: string,
     *   readMeHtml: string,
     *   files: [{ fileBytes: string, fileName: string }]
     * }
     * @param {Object} request
     * @param {number} request.productBarID
     * @param {string} request.message
     * @param {string|null} request.reportType
     * @param {string} request.readMe
     * @param {string} request.readMeHtml
     * @param {Array<{fileBytes:string,fileName:string}>} [request.files]
     * @param {string} token - Bearer token
     */
    async addProductReport(request) {
        try {
            if (!this.tokenStorage.isAuthorized()) {
                throw new Error('Unauthorized: Please login first');
            }

            const token = this.tokenStorage.getAccessToken();
            const body = {
                productBarID: request.productBarID || 0,
                message: request.message || '',
                reportType: request.reportType || '',
                readMe: request.readMe || '',
                readMeHtml: request.readMeHtml || '',
                files: (request.files || []).map(f => ({
                    fileBytes: f.fileBytes || '',
                    fileName: f.fileName || ''
                }))
            };

            const response = await axios.post(
                `${this.API_BASE_URL}/api/product/AddProductReport`,
                body,
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`📝 Product report added for ProductBarID: ${body.productBarID}`);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Add Rating
    /**
     * Adds a rating for a product.
     * @param {Object} request
     * @param {number} request.productBarID
     * @param {string} request.message
     * @param {number} request.rateCount
     * @param {Array<{fileBytes:string,fileName:string}>} [request.files]
     * @param {string} token - Bearer token
     * @returns {Promise<any>} API response
     */
    async addRating(request) {
        try {
            if (!this.tokenStorage.isAuthorized()) {
                throw new Error('Unauthorized: Please login first');
            }

            const token = this.tokenStorage.getAccessToken();
            const body = {
                productBarID: request.productBarID || 0,
                message: request.message || '',
                rateCount: request.rateCount || 0,
                files: (request.files || []).map(f => ({
                    fileBytes: f.fileBytes || '',
                    fileName: f.fileName || ''
                }))
            };

            const response = await axios.post(
                `${this.API_BASE_URL}/api/AddRating`,
                body,
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`⭐ Rating added for ProductBarID: ${body.productBarID}`);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Get Rating By Product
    /**
     * Retrieves rating(s) for a product by barID.
     * @param {Object|number} request - Either an object { barID } or a numeric barID
     * @returns {Promise<any>} rating response
     */
    async getRatingByProduct(request) {
        try {
            const barID = typeof request === 'number' ? request : (request?.barID || 0);

            const response = await axios.post(
                `${this.API_BASE_URL}/api/public/GetRatingByProduct`,
                { barID },
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`⭐ Ratings fetched for BarID: ${barID}`);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Create Checkout Session (Stripe)
    /**
     * Creates a Stripe Checkout session via the API.
     * @param {Object} request
     * @param {string} request.emailAddress
     * @param {number} request.addressBarID
     * @param {Array<{barId:number,slug:string,quantity:number,price:number}>} request.items
     * @param {string} [request.businessUnitKey] - optional; defaults to configured business unit key
     * @param {string} [token] - optional Bearer token
     * @returns {Promise<any>} { clientSecret: string } or server response
     */
    async createCheckoutSession(request) {
        try {
            const body = {
                emailAddress: request?.emailAddress || '',
                addressBarID: request?.addressBarID || 0,
                items: (request?.items || []).map(i => ({
                    barId: i.barId || i.barID || 0,
                    slug: i.slug || '',
                    quantity: i.quantity || 1,
                    price: i.price || 0
                })),
                businessUnitKey: request?.businessUnitKey || this.businessUnitKey
            };

            const headers = {
                'businessunitkey': this.businessUnitKey,
                'Content-Type': 'application/json'
            };
            
            if (this.tokenStorage.isAuthorized()) {
                headers.Authorization = `Bearer ${this.tokenStorage.getAccessToken()}`;
            }

            const response = await axios.post(
                `${this.API_BASE_URL}/api/public/checkout/create-session`,
                body,
                { headers }
            );

            console.log(`💳 Checkout session created (businessUnitKey: ${body.businessUnitKey})`);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Send (simulate) Stripe webhook (testing helper)
    /**
     * Sends a raw Stripe webhook JSON payload to the API webhook receiver.
     * This is intended as a testing helper to simulate Stripe sending events.
     * @param {string|Object} payloadJson - Raw JSON string or object to send as the request body
     * @param {string} signature - The value for the "Stripe-Signature" header (for verification)
     * @param {string} [token] - Optional Bearer token if your webhook endpoint requires it (not typical)
     * @returns {Promise<any>} Response from webhook endpoint
     */
    async sendStripeWebhook(payloadJson, signature) {
        try {
            const body = typeof payloadJson === 'string' ? payloadJson : JSON.stringify(payloadJson);

            const headers = {
                'businessunitkey': this.businessUnitKey,
                'Content-Type': 'application/json',
                'Stripe-Signature': signature || ''
            };

            if (this.tokenStorage.isAuthorized()) {
                headers.Authorization = `Bearer ${this.tokenStorage.getAccessToken()}`;
            }

            const response = await axios.post(
                `${this.API_BASE_URL}/api/stripe/webhook`,
                body,
                { headers }
            );

            console.log('🔔 Stripe webhook simulated');
            return response.data;
        } catch (error) {
            if (error.response?.status === 401) {
                this.tokenStorage.clearTokens();
                throw new Error('Unauthorized: Your session has expired. Please login again.');
            }
            this.handleError(error);
        }
    }

    // ✅ Get Product Details
    /**
     * Retrieve full product details by slug.
     * Mirrors the C# ProductRequest -> POST /api/public/ProductDetails
     * @param {Object} request
     * @param {string} request.slug - product slug
     * @param {string} [request.search] - optional search text
     * @param {string} [request.sortBy] - optional sort option
     * @param {number} [request.page=1]
     * @param {number} [request.pageSize=10]
     * @returns {Promise<any>} product details response
     */
    async getProductDetails(request) {
        try {
            const body = {
                slug: request?.slug || '',
                search: request?.search || '',
                sortBy: request?.sortBy || '',
                page: request?.page || 1,
                pageSize: request?.pageSize || 10
            };

            const response = await axios.post(
                `${this.API_BASE_URL}/api/public/ProductDetails`,
                body,
                {
                    headers: {
                        'businessunitkey': this.businessUnitKey,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`🔎 Product details fetched for slug: ${body.slug}`);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

// ✅ Get Report by Product
async getReportByProduct(barID) {
    try {
        const response = await axios.post(
            `${this.API_BASE_URL}/api/public/GetReportByProduct`,
            { barID }, // API expects this object
            {
                headers: {
                    'businessunitkey': this.businessUnitKey,
                    'Content-Type': 'application/json'
                }
            }
        );

        console.log(`📋 Retrieved reports for ProductBarID: ${barID}`);
        return response.data;
    } catch (error) {
        this.handleError(error);
    }
}


// ✅ Update Product Report
/**
 * Updates a product report
 * @param {Object} request - The update product report request
 * @param {number} request.reportBarID - The report bar ID (ulong in C#)
 * @param {string} request.message - The report message
 * @param {string|null} request.reportType - The type of report
 * @param {string} request.readMe - The readme content
 * @param {string} request.readMeHtml - The HTML formatted readme content
 * @param {Array<{fileBytes: string, fileName: string}>} [request.files] - Array of files with base64 content
 * @param {string} token - Bearer token for authentication
 * @returns {Promise<any>} The response from the server
 */
async updateProductReport(request) {
    try {
        if (!this.tokenStorage.isAuthorized()) {
            throw new Error('Unauthorized: Please login first');
        }

        const token = this.tokenStorage.getAccessToken();
        // Ensure files array is initialized if not provided
        const requestBody = {
            reportBarID: request.reportBarID || 0,
            message: request.message || '',
            reportType: request.reportType || '',
            readMe: request.readMe || '',
            readMeHtml: request.readMeHtml || '',
            files: (request.files || []).map(file => ({
                fileBytes: file.fileBytes || '',
                fileName: file.fileName || ''
            }))
        };

        const response = await axios.put(
            `${this.API_BASE_URL}/api/UpdateProductReport`,
            requestBody,
            {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'businessunitkey': this.businessUnitKey,
                    'Content-Type': 'application/json'
                }
            }
        );

        console.log(`📝 Report updated successfully (ReportBarID: ${request.reportBarID})`);
        return response.data;
    } catch (error) {
        this.handleError(error);
    }
}
// ✅ GET CART (requires Bearer token)
async getCart() {
    try {
        if (!this.tokenStorage.isAuthorized()) {
            throw new Error('Unauthorized: Please login first');
        }

        const token = this.tokenStorage.getAccessToken();
        const response = await axios.get(`${this.API_BASE_URL}/api/product/GetCart`, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'businessunitkey': this.businessUnitKey
            }
        });

        console.log('🛒 Cart items retrieved successfully:');
        console.log(JSON.stringify(response.data, null, 2));
        return response.data;
    } catch (error) {
        this.handleError(error);
    }
}
// ✅ CLEAR CART
async clearCart() {
    try {
        if (!this.tokenStorage.isAuthorized()) {
            throw new Error('Unauthorized: Please login first');
        }

        const token = this.tokenStorage.getAccessToken();
        const response = await axios.post(
            `${this.API_BASE_URL}/api/product/ClearCart`,
            {}, // no body needed
            {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'businessunitkey': this.businessUnitKey,
                    'Content-Type': 'application/json'
                }
            }
        );

        console.log('🧹 Cart cleared successfully!');
        return response.status; // Should return 204 (No Content)
    } catch (error) {
        this.handleError(error);
    }
}
async getAboutInfo() {
        try {
            const response = await axios.get(`${this.API_BASE_URL}/api/public/AboutInfo`, {
                headers: { "businessunitkey": this.businessUnitKey }
            });
            console.log("📘 About Info:", response.data);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Blogs
    async getBlogs() {
        try {
            const response = await axios.get(`${this.API_BASE_URL}/api/public/GetBlogs`, {
                headers: { "businessunitkey": this.businessUnitKey }
            });
            console.log("📰 Blogs:", response.data);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Documentation
    async getDocumentation() {
        try {
            const response = await axios.get(`${this.API_BASE_URL}/api/public/GetDocumentation`, {
                headers: { "businessunitkey": this.businessUnitKey }
            });
            console.log("📄 Documentation:", response.data);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Services
    async getServices() {
        try {
            const response = await axios.get(`${this.API_BASE_URL}/api/public/GetServices`, {
                headers: { "businessunitkey": this.businessUnitKey }
            });
            console.log("🛠️ Services:", response.data);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Get Address / Adres (no parameters)
    /**
     * Fetch address information from the API. Some backends may use "GetAddress" or "GetAdres".
     * This method tries both common routes: `/api/public/GetAddress` and `/api/public/GetAdres`.
     * It returns the first successful response.
     * @returns {Promise<any>} address data
     */
        async getAddress() {
        // Try the non-public routes first (matches API swagger screenshot)
        const paths = [
            `${this.API_BASE_URL}/api/GetAddress`,
        ];

        for (const url of paths) {
            try {
                const headers = { businessunitkey: this.businessUnitKey };
                    if (this.tokenStorage.isAuthorized()) {
                        headers.Authorization = `Bearer ${this.tokenStorage.getAccessToken()}`;
                    }

                const response = await axios.get(url, { headers });
                console.log(`📍 Address fetched from ${url}`);
                return response.data;
            } catch (err) {
                // If 404 or similar, try next; otherwise rethrow via handleError
                if (err.response && (err.response.status === 404 || err.response.status === 501)) {
                    // try next path
                    continue;
                }
                    if (err.response?.status === 401) {
                        this.tokenStorage.clearTokens();
                        throw new Error('Unauthorized: Your session has expired. Please login again.');
                    }
                this.handleError(err);
            }
        }

        // If neither path worked, throw a clear error
        throw new Error('Address endpoint not found at GetAddress or GetAdres');
    }
    
    // ✅ Upsert Address
    /**
     * Creates or updates an address
     * @param {Object} request - The address request object
     * @param {number} request.barID - The bar ID (ulong in C#)
     * @param {Object} request.addressDetails - The address details
     * @param {number} request.addressDetails.barID - The bar ID for the address
     * @param {string} request.addressDetails.email - Email address
     * @param {string} request.addressDetails.site - Site name
     * @param {string} request.addressDetails.phoneNumber - Phone number
     * @param {string} request.addressDetails.state - State
     * @param {string} request.addressDetails.pincode - Postal/PIN code
     * @param {string} request.addressDetails.country - Country
     * @param {string} request.addressDetails.address - Street address
     * @param {string} token - Bearer token for authentication
     * @returns {Promise<any>} Response from the server
     */
    async upsertAddress(request, token) {
        try {
            // Ensure all fields are present with defaults
            const requestBody = {
                barID: request.barID || 0,
                addressDetails: {
                    barID: request.addressDetails?.barID || request.barID || 0,
                    email: request.addressDetails?.email || '',
                    site: request.addressDetails?.site || '',
                    phoneNumber: request.addressDetails?.phoneNumber || '',
                    state: request.addressDetails?.state || '',
                    pincode: request.addressDetails?.pincode || '',
                    country: request.addressDetails?.country || '',
                    address: request.addressDetails?.address || ''
                }
            };

            const response = await axios.post(
                `${this.API_BASE_URL}/api/UpsertAddress`,
                requestBody,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'businessunitkey': this.businessUnitKey,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`📍 Address upserted for BarID: ${requestBody.barID}`);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Add Products to Cart
    /**
     * Adds or updates products in the user's shopping cart
     * @param {Object[]} cartItems - Array of cart items to add
     * @param {string} cartItems[].slug - Product slug/identifier
     * @param {number} cartItems[].quantity - Quantity to add
     * @param {string} token - Bearer token for authentication
     * @returns {Promise<Object>} Response with success message
     */
    async addProductsToCart(cartItems) {
        try {
            if (!this.tokenStorage.isAuthorized()) {
                throw new Error('Unauthorized: Please login first');
            }

            const token = this.tokenStorage.getAccessToken();
            const request = {
                cartItem: cartItems.map(item => ({
                    slug: item.slug || '',
                    quantity: item.quantity || 1
                }))
            };

            const response = await axios.post(
                `${this.API_BASE_URL}/api/product/AddProductsToCart`,
                request,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'businessunitkey': this.businessUnitKey,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log('🛒 Products added to cart successfully');
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Remove Product From Cart
    /**
     * Remove a product from the user's cart by slug.
     * @param {string} slug - product slug to remove
     * @param {string} token - Bearer token for authentication
     * @returns {Promise<Object|{status: number}>} API response or 204 status indicator
     */
    async removeProductFromCart(slug) {
        try {
            if (!this.tokenStorage.isAuthorized()) {
                throw new Error('Unauthorized: Please login first');
            }

            const token = this.tokenStorage.getAccessToken();
            const response = await axios.post(
                `${this.API_BASE_URL}/api/product/RemoveProductFromTheCart`,
                { slug },
                {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'businessunitkey': this.businessUnitKey,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`🗑️ Product removed from cart (slug: ${slug})`);
            // Some endpoints return 204 No Content — normalize to a simple object
            return response.status === 204 ? { status: 204 } : response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // ✅ Insert User Activity
    /**
     * Inserts a user activity record.
     * @param {Object} request
     * @param {string} request.userID
     * @param {number} request.businessUnitId
     * @param {string} request.eventType
     * @param {string} request.metaData
     * @param {string} token - Bearer token
     * @returns {Promise<any>} API response
     */
    async insertUserActivity(request, token) {
        try {
            const body = {
                userID: request.userID || '',
                businessUnitId: request.businessUnitId || request.businessUnitID || 0,
                eventType: request.eventType || '',
                metaData: request.metaData || ''
            };

            const response = await axios.post(
                `${this.API_BASE_URL}/api/product/InsertUserActivity`,
                body,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'businessunitkey': this.businessUnitKey,
                        'Content-Type': 'application/json'
                    }
                }
            );

            console.log(`📝 User activity inserted for userID: ${body.userID}`);
            return response.data;
        } catch (error) {
            this.handleError(error);
        }
    }

    // Common error handler
    handleError(error) {
        if (error.response) {
            console.error(`API Error: ${error.response.status}`);
            console.error(error.response.data);
            throw new Error(
                `API Error: ${error.response.status} - ${JSON.stringify(error.response.data)}`
            );
        } else if (error.request) {
            throw new Error(`No response received: ${error.message}`);
        } else {
            throw new Error(`Error: ${error.message}`);
        }
    }
}

export default ProductGroupClient;
