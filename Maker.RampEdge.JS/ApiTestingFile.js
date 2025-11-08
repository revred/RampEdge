import dotenv from 'dotenv';
import { ProductGroupClient } from "./src/index.js";

async function run() {
    try {
        console.log(" Starting test...");
        const client = new ProductGroupClient(process.env.BUSINESS_UNIT_KEY,process.env.API_BASE_URL);

        // Login once to get token
        const loginResponse = await client.login("demo@maker.ai", "test@123456789");
        console.log(" Login successful, token stored in tokenStorage");

        // Test multiple cancel orders using the same stored token
        console.log('\n🔄 Testing multiple order cancellations with stored token');
        
        // Array of test order IDs
        const orderIds = ['123456', '789012', '345678'];
        
        // Try to cancel multiple orders without logging in again
        for (const orderId of orderIds) {
            try {
                console.log(`\nTrying to cancel order: ${orderId}`);
                const result = await client.cancelOrder(orderId);
                console.log(`✅ Successfully cancelled order ${orderId}`);
                console.log('Response:', JSON.stringify(result, null, 2));
                
                // Verify token is still in storage
                const storedToken = client.tokenStorage.getAccessToken();
                console.log('Token still in storage:', !!storedToken);
                
            } catch (err) {
                if (err.response?.status === 404) {
                    console.log(`ℹ️ Order ${orderId} not found (expected for test IDs)`);
                } else if (err.message.includes('Unauthorized')) {
                    console.error(`❌ Token validation failed for order ${orderId}`);
                    break;
                } else {
                    console.error(`❌ Error cancelling order ${orderId}:`, err.message);
                }
            }
        }

        // Test cancelOrder
        console.log('\n🔄 Testing cancelOrder');
        
        // Test 1: Attempt to cancel order without login
        try {
            // Clear any existing tokens first
            client.tokenStorage.clearTokens();
            console.log('Testing cancelOrder without authorization...');
            await client.cancelOrder('123');
            console.log('❌ Test failed: Should have thrown unauthorized error');
        } catch (err) {
            if (err.message.includes('Unauthorized: Please login first')) {
                console.log('✅ Successfully caught unauthorized error when not logged in');
            } else {
                console.error('❌ Unexpected error:', err.message);
            }
        }

        // Test 2: Attempt to cancel order with valid token
        try {
            // Set the token back
            client.tokenStorage.setTokens(token, loginResponse?.refreshToken);
            console.log('Testing cancelOrder with valid token...');
            const barId = '123456'; // Replace with a valid barId if available
            const result = await client.cancelOrder(barId);
            console.log('✅ Cancel order response:', JSON.stringify(result, null, 2));
        } catch (err) {
            if (err.response?.status === 404) {
                console.log('ℹ️ Order not found (expected if using test barId)');
            } else {
                console.error('❌ Cancel order failed:', err.message);
            }
        }

        // Test 3: Test with expired/invalid token
        try {
            console.log('Testing cancelOrder with invalid token...');
            client.tokenStorage.setTokens('invalid_token', 'invalid_refresh_token');
            await client.cancelOrder('123');
            console.log('❌ Test failed: Should have thrown unauthorized error');
        } catch (err) {
            if (err.message.includes('Unauthorized: Your session has expired')) {
                console.log('✅ Successfully caught expired token error');
            } else {
                console.error('❌ Unexpected error:', err.message);
            }
        }



        // // Test getRatingByProduct (public)
        // console.log('\n📣 Testing getRatingByProduct');
        try {
            const ratingResp = await client.getRatingByProduct({ barID: 83796308744784 });
            console.log('✅ getRatingByProduct response:', JSON.stringify(ratingResp, null, 2));
        } catch (err) {
            console.error('❌ getRatingByProduct failed:', err.message || err);
        }

        // Test addRating (live)
        // console.log('\n⭐ Testing addRating (posting to live API)');
        // try {
        //     const ratingReq = {
        //         productBarID: 83796308744784,
        //         message: "publishTesting",
        //         rateCount: 5,
        //         files: [ { fileBytes: "", fileName: "" } ]
        //     };

        //     const addRatingResult = await client.addRating(ratingReq, token);
        //     console.log('✅ addRating response:', JSON.stringify(addRatingResult, null, 2));
        // } catch (err) {
        //     console.error('❌ addRating failed:', err.message || err);
        // }

    // Example: Request SSO Code (commented out)
    // Uncomment to test RequestSsoCode endpoint. Provide valid tokens.
    // try {
    //     const ssoReq = { refreshToken: 'your_refresh_token', accessToken: 'your_access_token' };
    //     const ssoResp = await client.requestSsoCode(ssoReq);
    //     console.log('✅ requestSsoCode response:', JSON.stringify(ssoResp, null, 2));
    // } catch (err) {
    //     console.error('❌ requestSsoCode failed:', err.message || err);
    // }

    // Test getProductDetails
        // console.log('\n🔎 Testing getProductDetails');
        // try {
        //     const productReq = {
        //         slug: "ZenDelta_Round96_Without_Mounting_Bores",
        //         search: "",
        //         sortBy: "",
        //         page: 1,
        //         pageSize: 10
        //     };

        //     const productDetails = await client.getProductDetails(productReq);
        //     console.log('✅ getProductDetails response:', JSON.stringify(productDetails, null, 2));
        // } catch (err) {
        //     console.error('❌ getProductDetails failed:', err.message || err);
        // }

    // Example: Create Checkout Session (commented out by default)
    // WARNING: This will create a real Stripe Checkout session when enabled.
    // Uncomment and provide valid items/email if you want to test.
    // try {
    //     const checkoutReq = {
    //         emailAddress: 'string',
    //         addressBarID: 0,
    //         items: [
    //             { barId: 0, slug: 'string', quantity: 1, price: 100 }
    //         ],
    //         businessUnitKey: process.env.BUSINESS_UNIT_KEY || ''
    //     };
    //     const session = await client.createCheckoutSession(checkoutReq, /* token? */ undefined);
    //     console.log('✅ createCheckoutSession response:', JSON.stringify(session, null, 2));
    // } catch (err) {
    //     console.error('❌ createCheckoutSession failed:', err.message || err);
    // }

        // // Test removeProductFromCart
        // console.log('\n🗑️ Testing removeProductFromCart');
        // try {
        //     const removeResult = await client.removeProductFromCart("ZenDelta_Round96_Without_Mounting_Bores", token);
        //     console.log('✅ removeProductFromCart response:', JSON.stringify(removeResult, null, 2));
        // } catch (err) {
        //     console.error('❌ removeProductFromCart failed:', err.message || err);
        // }

      //  --- Active test: call getAddress/getAdres (no-parameter endpoint)
        // console.log('\n🧭 Testing getAddress/getAdres (no parameters)');
        // try {
        //     const addressData = await client.getAddress(token);
        //     console.log('✅ getAddress response:', JSON.stringify(addressData, null, 2));
        // } catch (err) {
        //     console.error('❌ getAddress failed:', err.message || err);
        // }

        // Test upsertAddress
        // console.log('\n🔄 Testing upsertAddress');
        // try {
        //     const addressRequest = {
        //         barID: 12345,
        //         addressDetails: {
        //             barID: 12345,
        //             email: "test@example.com",
        //             site: "Test Site",
        //             phoneNumber: "1234567890",
        //             state: "Test State",
        //             pincode: "123456",
        //             country: "Test Country",
        //             address: "123 Test Street"
        //         }
        //     };
            
        //     const upsertResult = await client.upsertAddress(addressRequest, token);
        //     console.log('✅ upsertAddress response:', JSON.stringify(upsertResult, null, 2));
        // } catch (err) {
        //     console.error('❌ upsertAddress failed:', err.message || err);
        // }

        // Test adding products to cart
        // console.log('\n🛒 Testing addProductsToCart');
        // try {
        //     const cartItems = [{
        //         slug: "ZenDelta_Round96_Without_Mounting_Bores",
        //         quantity: 1
        //     }];
            
        //     const cartResult = await client.addProductsToCart(cartItems, token);
        //     console.log('✅ addProductsToCart response:', JSON.stringify(cartResult, null, 2));
        // } catch (err) {
        //     console.error('❌ addProductsToCart failed:', err.message || err);
        // }

    // Example: Simulate Stripe webhook (commented out)
    // WARNING: Only enable if you want to POST a test webhook to the live endpoint.
    // const sampleEvent = {
    //     id: 'evt_test_webhook',
    //     object: 'event',
    //     type: 'checkout.session.completed',
    //     data: {
    //         object: {
    //             id: 'cs_test',
    //             object: 'checkout.session',
    //             metadata: { checkout_request: JSON.stringify({ /* your CreateSession payload */ }) }
    //         }
    //     }
    // };
    // const fakeSig = 't=123456,v1=fake_signature';
    // try {
    //     const webhookResp = await client.sendStripeWebhook(sampleEvent, fakeSig);
    //     console.log('✅ sendStripeWebhook response:', JSON.stringify(webhookResp, null, 2));
    // } catch (err) {
    //     console.error('❌ sendStripeWebhook failed:', err.message || err);
    // }

        // Example: Insert User Activity (commented out)
        // This posts a user activity. Enable only when you want to record a real activity.
        // try {
        //     const activity = {
        //         userID: 'user-123',
        //         businessUnitId: 0,
        //         eventType: 'ProductView',
        //         metaData: JSON.stringify({ slug: 'ZenDelta_Round96_Without_Mounting_Bores' })
        //     };
        //     const activityResp = await client.insertUserActivity(activity, token);
        //     console.log('✅ insertUserActivity response:', JSON.stringify(activityResp, null, 2));
        // } catch (err) {
        //     console.error('❌ insertUserActivity failed:', err.message || err);
        // }

        // Create and send AddProductReport request (matches C# AddProductReportRequest)
        // const addRequest = {
        //     productBarID: 83796308744784,
        //     message: "Testing for console app",
        //     reportType: "",
        //     readMe: "fgfdgg",
        //     readMeHtml: "dgfdg",
        //     files: [
        //         {
        //             fileBytes: "", // put base64 file contents here if needed
        //             fileName: ""
        //         }
        //     ]
        // };

      

          // const updateRequest = {
        //     reportBarID: 59653644629301,
        //     message: "Updated issue details message",
        //     reportType: "BugFix",
        //     readMe: "ReadMe updated text",
        //     readMeHtml: "<p>Updated HTML ReadMe</p>",
        //     files: [
        //         {
        //             fileBytes: "", // Base64 encoded "Hello World"
        //             fileName: ""
        //         }
        //     ]
        // };

        // console.log(" Sending update request...");
        // const result = await client.updateProductReport(updateRequest, token);
        // console.log(" Update successful:", result);

    } catch (error) {
        console.error(" Test failed:", error.message);
        process.exit(1);
    }
}

// Run the test
run().catch(error => {
    console.error(" Unhandled error:", error);
    process.exit(1);
});
