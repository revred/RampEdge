import { jest } from '@jest/globals';
import axios from 'axios';
import { ProductGroupClient } from '../index.js';

let mockTokenStorage;
      describe('ProductGroupClient', () => {
        let client;

        beforeEach(() => {
          // reset manual axios spies
          axios.get = jest.fn();
          axios.post = jest.fn();
          axios.put = jest.fn();
          axios.delete = jest.fn();
          jest.clearAllMocks();

          mockTokenStorage = {
            setTokens: jest.fn(),
            getAccessToken: jest.fn(),
            getRefreshToken: jest.fn(),
            clearTokens: jest.fn(),
            isAuthorized: jest.fn()
          };

          client = new ProductGroupClient('test-key', 'https://api.example.com');
          client.tokenStorage = mockTokenStorage;
        });

        test('registerUser calls axios.post with correct params', async () => {
          axios.post.mockResolvedValue({ data: { success: true } });
          const result = await client.registerUser('user@example.com', 'pass');
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/public/authentication/Register',
            { email: 'user@example.com', password: 'pass' },
            expect.objectContaining({ headers: expect.any(Object) })
          );
          expect(result).toEqual({ success: true });
        });

        test('login stores tokens on success', async () => {
          axios.post.mockResolvedValue({ data: { token: 'abc', refreshToken: 'def' } });
          await client.login('user', 'pass');
          expect(mockTokenStorage.setTokens).toHaveBeenCalledWith('abc', 'def');
        });

        test('refreshToken uses refresh token and stores new tokens', async () => {
          mockTokenStorage.getRefreshToken.mockReturnValue('refresh123');
          axios.post.mockResolvedValue({ data: { token: 'newtok', refreshToken: 'newref' } });
          const res = await client.refreshToken();
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/public/authentication/refresh-token',
            { refreshToken: 'refresh123' },
            expect.objectContaining({ headers: expect.any(Object) })
          );
          expect(mockTokenStorage.setTokens).toHaveBeenCalledWith('newtok', 'newref');
          expect(res).toEqual({ token: 'newtok', refreshToken: 'newref' });
        });

        test('cancelOrder requires auth and posts to OrderCancel', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.post.mockResolvedValue({ data: { ok: true } });
          const res = await client.cancelOrder('123');
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/product/OrderCancel',
            { barId: '123' },
            expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) })
          );
          expect(res).toEqual({ ok: true });
        });

  test('requestSsoCode uses stored tokens and requires auth', async () => {
    mockTokenStorage.isAuthorized.mockReturnValue(true);
    mockTokenStorage.getRefreshToken.mockReturnValue('stored-refresh');
    mockTokenStorage.getAccessToken.mockReturnValue('stored-access');
    axios.post.mockResolvedValue({ data: { code: 'abc' } });
    
    const res = await client.requestSsoCode();
    
    expect(mockTokenStorage.isAuthorized).toHaveBeenCalled();
    expect(mockTokenStorage.getRefreshToken).toHaveBeenCalled();
    expect(mockTokenStorage.getAccessToken).toHaveBeenCalled();
    expect(axios.post).toHaveBeenCalledWith(
      'https://api.example.com/api/public/authentication/RequestSsoCode',
      { refreshToken: 'stored-refresh', accessToken: 'stored-access' },
      expect.objectContaining({ headers: expect.any(Object) })
    );
    expect(res).toEqual({ code: 'abc' });
  });

  test('requestSsoCode throws if not authorized', async () => {
    mockTokenStorage.isAuthorized.mockReturnValue(false);
    await expect(client.requestSsoCode()).rejects.toThrow('Unauthorized: Please login first');
  });        test('getAllOrders requires auth and posts to GetAllOrders', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.post.mockResolvedValue({ data: { orders: [] } });
          const res = await client.getAllOrders(1, 5);
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/product/GetAllOrders',
            { page: 1, pageSize: 5 },
            expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) })
          );
          expect(res).toEqual({ orders: [] });
        });

        test('getProductGroups calls public ProductGroups', async () => {
          axios.get.mockResolvedValue({ data: { groups: ['A', 'B'] } });
          const result = await client.getProductGroups();
          expect(axios.get).toHaveBeenCalledWith(
            'https://api.example.com/api/public/ProductGroups',
            expect.objectContaining({ headers: expect.objectContaining({ businessunitkey: 'test-key' }) })
          );
          expect(result).toEqual({ groups: ['A', 'B'] });
        });

        test('getProductsBySlug posts slug', async () => {
          axios.post.mockResolvedValue({ data: { products: [] } });
          const res = await client.getProductsBySlug('slug');
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/public/ProductsBySlug',
            { slug: 'slug' },
            expect.objectContaining({ headers: expect.any(Object) })
          );
          expect(res).toEqual({ products: [] });
        });

        test('getUser requires auth and posts to /api/entity/User', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.post.mockResolvedValue({ data: { user: {} } });
          const res = await client.getUser('detail');
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/entity/User',
            { userDetail: 'detail' },
            expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) })
          );
          expect(res).toEqual({ user: {} });
        });

        test('addProductReport requires auth and posts AddProductReport', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.post.mockResolvedValue({ data: { ok: true } });
          const req = { productBarID: 1, message: 'm', files: [] };
          const res = await client.addProductReport(req);
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/product/AddProductReport',
            expect.any(Object),
            expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) })
          );
          expect(res).toEqual({ ok: true });
        });

        test('addRating requires auth and posts AddRating', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.post.mockResolvedValue({ data: { ok: true } });
          const req = { productBarID: 1, message: 'm', rateCount: 5, files: [] };
          const res = await client.addRating(req);
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/AddRating',
            expect.any(Object),
            expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) })
          );
          expect(res).toEqual({ ok: true });
        });

        test('getRatingByProduct posts to public/GetRatingByProduct', async () => {
          axios.post.mockResolvedValue({ data: { ratings: [] } });
          const res = await client.getRatingByProduct({ barID: 1 });
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/public/GetRatingByProduct',
            { barID: 1 },
            expect.objectContaining({ headers: expect.any(Object) })
          );
          expect(res).toEqual({ ratings: [] });
        });

        test('createCheckoutSession posts to create-session and includes auth when present', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.post.mockResolvedValue({ data: { clientSecret: 'cs' } });
          const req = { emailAddress: 'e', addressBarID: 0, items: [] };
          const res = await client.createCheckoutSession(req);
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/public/checkout/create-session',
            expect.any(Object),
            expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) })
          );
          expect(res).toEqual({ clientSecret: 'cs' });
        });

        test('sendStripeWebhook posts raw body with Stripe-Signature header', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(false);
          axios.post.mockResolvedValue({ data: { ok: true } });
          const payload = { id: 'evt' };
          const res = await client.sendStripeWebhook(payload, 'sig');
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/stripe/webhook',
            JSON.stringify(payload),
            expect.objectContaining({ headers: expect.objectContaining({ 'Stripe-Signature': 'sig' }) })
          );
          expect(res).toEqual({ ok: true });
        });

        test('getProductDetails posts to ProductDetails', async () => {
          axios.post.mockResolvedValue({ data: { detail: {} } });
          const res = await client.getProductDetails({ slug: 's' });
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/public/ProductDetails',
            expect.any(Object),
            expect.objectContaining({ headers: expect.any(Object) })
          );
          expect(res).toEqual({ detail: {} });
        });

        test('getReportByProduct posts to GetReportByProduct', async () => {
          axios.post.mockResolvedValue({ data: { reports: [] } });
          const res = await client.getReportByProduct(5);
          expect(axios.post).toHaveBeenCalledWith(
            'https://api.example.com/api/public/GetReportByProduct',
            { barID: 5 },
            expect.objectContaining({ headers: expect.any(Object) })
          );
          expect(res).toEqual({ reports: [] });
        });

        test('updateProductReport requires auth and puts to UpdateProductReport', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.put.mockResolvedValue({ data: { ok: true } });
          const req = { reportBarID: 1, message: 'm', files: [] };
          const res = await client.updateProductReport(req);
          expect(axios.put).toHaveBeenCalledWith(
            'https://api.example.com/api/UpdateProductReport',
            expect.any(Object),
            expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) })
          );
          expect(res).toEqual({ ok: true });
        });

        test('getCart requires auth and gets GetCart', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.get.mockResolvedValue({ data: { cart: [] } });
          const res = await client.getCart();
          expect(axios.get).toHaveBeenCalledWith('https://api.example.com/api/product/GetCart', expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) }));
          expect(res).toEqual({ cart: [] });
        });

        test('clearCart posts to ClearCart when authorized', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.post.mockResolvedValue({ status: 204 });
          const res = await client.clearCart();
          expect(axios.post).toHaveBeenCalledWith('https://api.example.com/api/product/ClearCart', {}, expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) }));
          expect(res).toEqual(204);
        });

        test('getAboutInfo/getBlogs/getDocumentation/getServices call public endpoints', async () => {
          axios.get.mockResolvedValueOnce({ data: { about: true } });
          const a = await client.getAboutInfo();
          expect(a).toEqual({ about: true });

          axios.get.mockResolvedValueOnce({ data: { blogs: [] } });
          const b = await client.getBlogs();
          expect(b).toEqual({ blogs: [] });

          axios.get.mockResolvedValueOnce({ data: { docs: [] } });
          const d = await client.getDocumentation();
          expect(d).toEqual({ docs: [] });

          axios.get.mockResolvedValueOnce({ data: { services: [] } });
          const s = await client.getServices();
          expect(s).toEqual({ services: [] });
        });

        test('getAddress tries GetAddress and returns data', async () => {
          axios.get.mockResolvedValue({ data: { address: {} } });
          const res = await client.getAddress();
          expect(res).toEqual({ address: {} });
        });

        test('upsertAddress posts to UpsertAddress with provided token', async () => {
          axios.post.mockResolvedValue({ data: { ok: true } });
          const res = await client.upsertAddress({ barID: 1, addressDetails: {} }, 'mytoken');
          expect(axios.post).toHaveBeenCalledWith('https://api.example.com/api/UpsertAddress', expect.any(Object), expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer mytoken' }) }));
          expect(res).toEqual({ ok: true });
        });

        test('addProductsToCart requires auth and posts to AddProductsToCart', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.post.mockResolvedValue({ data: { ok: true } });
          const res = await client.addProductsToCart([{ slug: 's', quantity: 1 }]);
          expect(axios.post).toHaveBeenCalledWith('https://api.example.com/api/product/AddProductsToCart', expect.any(Object), expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) }));
          expect(res).toEqual({ ok: true });
        });

        test('removeProductFromCart requires auth and posts to RemoveProductFromTheCart', async () => {
          mockTokenStorage.isAuthorized.mockReturnValue(true);
          mockTokenStorage.getAccessToken.mockReturnValue('tok');
          axios.post.mockResolvedValue({ data: { ok: true } });
          const res = await client.removeProductFromCart('slug');
          expect(axios.post).toHaveBeenCalledWith('https://api.example.com/api/product/RemoveProductFromTheCart', { slug: 'slug' }, expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer tok' }) }));
          expect(res).toEqual({ ok: true });
        });

        test('insertUserActivity posts with token param', async () => {
          axios.post.mockResolvedValue({ data: { ok: true } });
          const res = await client.insertUserActivity({ userID: 'u' }, 'mytoken');
          expect(axios.post).toHaveBeenCalledWith('https://api.example.com/api/product/InsertUserActivity', expect.any(Object), expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer mytoken' }) }));
          expect(res).toEqual({ ok: true });
        });
      });
