function _typeof(o) { "@babel/helpers - typeof"; return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (o) { return typeof o; } : function (o) { return o && "function" == typeof Symbol && o.constructor === Symbol && o !== Symbol.prototype ? "symbol" : typeof o; }, _typeof(o); }
function _regenerator() { /*! regenerator-runtime -- Copyright (c) 2014-present, Facebook, Inc. -- license (MIT): https://github.com/babel/babel/blob/main/packages/babel-helpers/LICENSE */ var e, t, r = "function" == typeof Symbol ? Symbol : {}, n = r.iterator || "@@iterator", o = r.toStringTag || "@@toStringTag"; function i(r, n, o, i) { var c = n && n.prototype instanceof Generator ? n : Generator, u = Object.create(c.prototype); return _regeneratorDefine2(u, "_invoke", function (r, n, o) { var i, c, u, f = 0, p = o || [], y = !1, G = { p: 0, n: 0, v: e, a: d, f: d.bind(e, 4), d: function d(t, r) { return i = t, c = 0, u = e, G.n = r, a; } }; function d(r, n) { for (c = r, u = n, t = 0; !y && f && !o && t < p.length; t++) { var o, i = p[t], d = G.p, l = i[2]; r > 3 ? (o = l === n) && (u = i[(c = i[4]) ? 5 : (c = 3, 3)], i[4] = i[5] = e) : i[0] <= d && ((o = r < 2 && d < i[1]) ? (c = 0, G.v = n, G.n = i[1]) : d < l && (o = r < 3 || i[0] > n || n > l) && (i[4] = r, i[5] = n, G.n = l, c = 0)); } if (o || r > 1) return a; throw y = !0, n; } return function (o, p, l) { if (f > 1) throw TypeError("Generator is already running"); for (y && 1 === p && d(p, l), c = p, u = l; (t = c < 2 ? e : u) || !y;) { i || (c ? c < 3 ? (c > 1 && (G.n = -1), d(c, u)) : G.n = u : G.v = u); try { if (f = 2, i) { if (c || (o = "next"), t = i[o]) { if (!(t = t.call(i, u))) throw TypeError("iterator result is not an object"); if (!t.done) return t; u = t.value, c < 2 && (c = 0); } else 1 === c && (t = i["return"]) && t.call(i), c < 2 && (u = TypeError("The iterator does not provide a '" + o + "' method"), c = 1); i = e; } else if ((t = (y = G.n < 0) ? u : r.call(n, G)) !== a) break; } catch (t) { i = e, c = 1, u = t; } finally { f = 1; } } return { value: t, done: y }; }; }(r, o, i), !0), u; } var a = {}; function Generator() {} function GeneratorFunction() {} function GeneratorFunctionPrototype() {} t = Object.getPrototypeOf; var c = [][n] ? t(t([][n]())) : (_regeneratorDefine2(t = {}, n, function () { return this; }), t), u = GeneratorFunctionPrototype.prototype = Generator.prototype = Object.create(c); function f(e) { return Object.setPrototypeOf ? Object.setPrototypeOf(e, GeneratorFunctionPrototype) : (e.__proto__ = GeneratorFunctionPrototype, _regeneratorDefine2(e, o, "GeneratorFunction")), e.prototype = Object.create(u), e; } return GeneratorFunction.prototype = GeneratorFunctionPrototype, _regeneratorDefine2(u, "constructor", GeneratorFunctionPrototype), _regeneratorDefine2(GeneratorFunctionPrototype, "constructor", GeneratorFunction), GeneratorFunction.displayName = "GeneratorFunction", _regeneratorDefine2(GeneratorFunctionPrototype, o, "GeneratorFunction"), _regeneratorDefine2(u), _regeneratorDefine2(u, o, "Generator"), _regeneratorDefine2(u, n, function () { return this; }), _regeneratorDefine2(u, "toString", function () { return "[object Generator]"; }), (_regenerator = function _regenerator() { return { w: i, m: f }; })(); }
function _regeneratorDefine2(e, r, n, t) { var i = Object.defineProperty; try { i({}, "", {}); } catch (e) { i = 0; } _regeneratorDefine2 = function _regeneratorDefine(e, r, n, t) { function o(r, n) { _regeneratorDefine2(e, r, function (e) { return this._invoke(r, n, e); }); } r ? i ? i(e, r, { value: n, enumerable: !t, configurable: !t, writable: !t }) : e[r] = n : (o("next", 0), o("throw", 1), o("return", 2)); }, _regeneratorDefine2(e, r, n, t); }
function asyncGeneratorStep(n, t, e, r, o, a, c) { try { var i = n[a](c), u = i.value; } catch (n) { return void e(n); } i.done ? t(u) : Promise.resolve(u).then(r, o); }
function _asyncToGenerator(n) { return function () { var t = this, e = arguments; return new Promise(function (r, o) { var a = n.apply(t, e); function _next(n) { asyncGeneratorStep(a, r, o, _next, _throw, "next", n); } function _throw(n) { asyncGeneratorStep(a, r, o, _next, _throw, "throw", n); } _next(void 0); }); }; }
function _classCallCheck(a, n) { if (!(a instanceof n)) throw new TypeError("Cannot call a class as a function"); }
function _defineProperties(e, r) { for (var t = 0; t < r.length; t++) { var o = r[t]; o.enumerable = o.enumerable || !1, o.configurable = !0, "value" in o && (o.writable = !0), Object.defineProperty(e, _toPropertyKey(o.key), o); } }
function _createClass(e, r, t) { return r && _defineProperties(e.prototype, r), t && _defineProperties(e, t), Object.defineProperty(e, "prototype", { writable: !1 }), e; }
function _toPropertyKey(t) { var i = _toPrimitive(t, "string"); return "symbol" == _typeof(i) ? i : i + ""; }
function _toPrimitive(t, r) { if ("object" != _typeof(t) || !t) return t; var e = t[Symbol.toPrimitive]; if (void 0 !== e) { var i = e.call(t, r || "default"); if ("object" != _typeof(i)) return i; throw new TypeError("@@toPrimitive must return a primitive value."); } return ("string" === r ? String : Number)(t); }
import axios from 'axios';
import dotenv from 'dotenv';
import { tokenStorage } from './tokenStorage.js';
dotenv.config(); // Load .env values

export var ProductGroupClient = /*#__PURE__*/function () {
  function ProductGroupClient() {
    var businessUnitKey = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : null;
    var apiBaseUrl = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : null;
    _classCallCheck(this, ProductGroupClient);
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
  return _createClass(ProductGroupClient, [{
    key: "registerUser",
    value: function () {
      var _registerUser = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee(email, password) {
        var response, _t;
        return _regenerator().w(function (_context) {
          while (1) switch (_context.p = _context.n) {
            case 0:
              _context.p = 0;
              _context.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/public/authentication/Register"), {
                email: email,
                password: password
              }, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 1:
              response = _context.v;
              console.log("\uD83E\uDDFE User registered successfully: ".concat(email));
              return _context.a(2, response.data);
            case 2:
              _context.p = 2;
              _t = _context.v;
              this.handleError(_t);
            case 3:
              return _context.a(2);
          }
        }, _callee, this, [[0, 2]]);
      }));
      function registerUser(_x, _x2) {
        return _registerUser.apply(this, arguments);
      }
      return registerUser;
    }() // ✅ LOGIN
  }, {
    key: "login",
    value: function () {
      var _login = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee2(username, password) {
        var _response$data, emailAndPassword, response, _t2;
        return _regenerator().w(function (_context2) {
          while (1) switch (_context2.p = _context2.n) {
            case 0:
              _context2.p = 0;
              // Combine email and password with '|'
              emailAndPassword = "".concat(username, "|").concat(password);
              _context2.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/public/Login"), {
                emailAndPassword: emailAndPassword
              },
              // body parameter
              {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 1:
              response = _context2.v;
              if (!((_response$data = response.data) !== null && _response$data !== void 0 && _response$data.token)) {
                _context2.n = 2;
                break;
              }
              this.tokenStorage.setTokens(response.data.token, response.data.refreshToken);
              console.log('✅ Login successful and tokens stored');
              _context2.n = 3;
              break;
            case 2:
              throw new Error('No token received from server');
            case 3:
              return _context2.a(2, response.data);
            case 4:
              _context2.p = 4;
              _t2 = _context2.v;
              this.tokenStorage.clearTokens(); // Clear any existing tokens on login failure
              this.handleError(_t2);
            case 5:
              return _context2.a(2);
          }
        }, _callee2, this, [[0, 4]]);
      }));
      function login(_x3, _x4) {
        return _login.apply(this, arguments);
      }
      return login;
    }() // ✅ Refresh Token
  }, {
    key: "refreshToken",
    value: function () {
      var _refreshToken = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee3() {
        var _response$data2, refreshTokenValue, response, _t3;
        return _regenerator().w(function (_context3) {
          while (1) switch (_context3.p = _context3.n) {
            case 0:
              _context3.p = 0;
              refreshTokenValue = this.tokenStorage.getRefreshToken();
              if (refreshTokenValue) {
                _context3.n = 1;
                break;
              }
              throw new Error('No refresh token available. Please login again.');
            case 1:
              _context3.n = 2;
              return axios.post("".concat(this.API_BASE_URL, "/api/public/authentication/refresh-token"), {
                refreshToken: refreshTokenValue
              }, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 2:
              response = _context3.v;
              if (!((_response$data2 = response.data) !== null && _response$data2 !== void 0 && _response$data2.token)) {
                _context3.n = 3;
                break;
              }
              this.tokenStorage.setTokens(response.data.token, response.data.refreshToken);
              console.log('🔄 Token refreshed successfully');
              _context3.n = 4;
              break;
            case 3:
              throw new Error('No token received from server');
            case 4:
              return _context3.a(2, response.data);
            case 5:
              _context3.p = 5;
              _t3 = _context3.v;
              this.tokenStorage.clearTokens(); // Clear tokens on refresh failure
              this.handleError(_t3);
            case 6:
              return _context3.a(2);
          }
        }, _callee3, this, [[0, 5]]);
      }));
      function refreshToken() {
        return _refreshToken.apply(this, arguments);
      }
      return refreshToken;
    }() // ✅ Cancel Product Order
  }, {
    key: "cancelOrder",
    value: function () {
      var _cancelOrder = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee4(barId) {
        var token, response, _error$response, _t4;
        return _regenerator().w(function (_context4) {
          while (1) switch (_context4.p = _context4.n) {
            case 0:
              _context4.p = 0;
              if (this.tokenStorage.isAuthorized()) {
                _context4.n = 1;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 1:
              token = this.tokenStorage.getAccessToken();
              _context4.n = 2;
              return axios.post("".concat(this.API_BASE_URL, "/api/product/OrderCancel"), {
                barId: barId
              },
              // request body
              {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Authorization': "Bearer ".concat(token),
                  'Content-Type': 'application/json'
                }
              });
            case 2:
              response = _context4.v;
              console.log("\uD83D\uDED1 Order with BarId ".concat(barId, " cancelled successfully."));
              return _context4.a(2, response.data);
            case 3:
              _context4.p = 3;
              _t4 = _context4.v;
              if (!(((_error$response = _t4.response) === null || _error$response === void 0 ? void 0 : _error$response.status) === 401)) {
                _context4.n = 4;
                break;
              }
              this.tokenStorage.clearTokens(); // Clear tokens on unauthorized response
              throw new Error('Unauthorized: Your session has expired. Please login again.');
            case 4:
              this.handleError(_t4);
            case 5:
              return _context4.a(2);
          }
        }, _callee4, this, [[0, 3]]);
      }));
      function cancelOrder(_x5) {
        return _cancelOrder.apply(this, arguments);
      }
      return cancelOrder;
    }() // ✅ Request SSO Code
    /**
     * Exchanges refresh/access tokens for a short-lived SSO code (valid ~3 minutes)
     * @param {Object} request - { refreshToken, accessToken }
     * @returns {Promise<any>} { code: string } or API error
     */
  }, {
    key: "requestSsoCode",
    value: function () {
      var _requestSsoCode = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee5(request) {
        var body, response, _t5;
        return _regenerator().w(function (_context5) {
          while (1) switch (_context5.p = _context5.n) {
            case 0:
              _context5.p = 0;
              body = {
                refreshToken: (request === null || request === void 0 ? void 0 : request.refreshToken) || '',
                accessToken: (request === null || request === void 0 ? void 0 : request.accessToken) || ''
              };
              _context5.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/public/authentication/RequestSsoCode"), body, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 1:
              response = _context5.v;
              console.log("\uD83D\uDD10 Requested SSO code");
              return _context5.a(2, response.data);
            case 2:
              _context5.p = 2;
              _t5 = _context5.v;
              this.handleError(_t5);
            case 3:
              return _context5.a(2);
          }
        }, _callee5, this, [[0, 2]]);
      }));
      function requestSsoCode(_x6) {
        return _requestSsoCode.apply(this, arguments);
      }
      return requestSsoCode;
    }() // ✅ Get All Orders
  }, {
    key: "getAllOrders",
    value: function () {
      var _getAllOrders = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee6() {
        var page,
          pageSize,
          token,
          response,
          _error$response2,
          _args6 = arguments,
          _t6;
        return _regenerator().w(function (_context6) {
          while (1) switch (_context6.p = _context6.n) {
            case 0:
              page = _args6.length > 0 && _args6[0] !== undefined ? _args6[0] : 0;
              pageSize = _args6.length > 1 && _args6[1] !== undefined ? _args6[1] : 10;
              _context6.p = 1;
              if (this.tokenStorage.isAuthorized()) {
                _context6.n = 2;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 2:
              token = this.tokenStorage.getAccessToken();
              _context6.n = 3;
              return axios.post("".concat(this.API_BASE_URL, "/api/product/GetAllOrders"), {
                page: page,
                pageSize: pageSize
              }, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Authorization': "Bearer ".concat(token),
                  'Content-Type': 'application/json'
                }
              });
            case 3:
              response = _context6.v;
              console.log('📦 Orders fetched successfully');
              return _context6.a(2, response.data);
            case 4:
              _context6.p = 4;
              _t6 = _context6.v;
              if (!(((_error$response2 = _t6.response) === null || _error$response2 === void 0 ? void 0 : _error$response2.status) === 401)) {
                _context6.n = 5;
                break;
              }
              this.tokenStorage.clearTokens();
              throw new Error('Unauthorized: Your session has expired. Please login again.');
            case 5:
              this.handleError(_t6);
            case 6:
              return _context6.a(2);
          }
        }, _callee6, this, [[1, 4]]);
      }));
      function getAllOrders() {
        return _getAllOrders.apply(this, arguments);
      }
      return getAllOrders;
    }() // ✅ Get all product groups
  }, {
    key: "getProductGroups",
    value: function () {
      var _getProductGroups = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee7() {
        var response, _t7;
        return _regenerator().w(function (_context7) {
          while (1) switch (_context7.p = _context7.n) {
            case 0:
              _context7.p = 0;
              _context7.n = 1;
              return axios.get("".concat(this.API_BASE_URL, "/api/public/ProductGroups"), {
                headers: {
                  'businessunitkey': this.businessUnitKey
                }
              });
            case 1:
              response = _context7.v;
              return _context7.a(2, response.data);
            case 2:
              _context7.p = 2;
              _t7 = _context7.v;
              this.handleError(_t7);
            case 3:
              return _context7.a(2);
          }
        }, _callee7, this, [[0, 2]]);
      }));
      function getProductGroups() {
        return _getProductGroups.apply(this, arguments);
      }
      return getProductGroups;
    }() // ✅ Get products by slug
  }, {
    key: "getProductsBySlug",
    value: function () {
      var _getProductsBySlug = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee8(slug) {
        var response, _t8;
        return _regenerator().w(function (_context8) {
          while (1) switch (_context8.p = _context8.n) {
            case 0:
              _context8.p = 0;
              _context8.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/public/ProductsBySlug"), {
                slug: slug
              }, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 1:
              response = _context8.v;
              return _context8.a(2, response.data);
            case 2:
              _context8.p = 2;
              _t8 = _context8.v;
              this.handleError(_t8);
            case 3:
              return _context8.a(2);
          }
        }, _callee8, this, [[0, 2]]);
      }));
      function getProductsBySlug(_x7) {
        return _getProductsBySlug.apply(this, arguments);
      }
      return getProductsBySlug;
    }() // ✅ Get User Details
  }, {
    key: "getUser",
    value: function () {
      var _getUser = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee9(userDetail) {
        var token, response, _error$response3, _t9;
        return _regenerator().w(function (_context9) {
          while (1) switch (_context9.p = _context9.n) {
            case 0:
              _context9.p = 0;
              if (this.tokenStorage.isAuthorized()) {
                _context9.n = 1;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 1:
              token = this.tokenStorage.getAccessToken();
              _context9.n = 2;
              return axios.post("".concat(this.API_BASE_URL, "/api/entity/User"), {
                userDetail: userDetail
              }, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Authorization': "Bearer ".concat(token),
                  'Content-Type': 'application/json'
                }
              });
            case 2:
              response = _context9.v;
              console.log("\uD83D\uDC64 User details fetched successfully for: ".concat(userDetail));
              return _context9.a(2, response.data);
            case 3:
              _context9.p = 3;
              _t9 = _context9.v;
              if (!(((_error$response3 = _t9.response) === null || _error$response3 === void 0 ? void 0 : _error$response3.status) === 401)) {
                _context9.n = 4;
                break;
              }
              this.tokenStorage.clearTokens();
              throw new Error('Unauthorized: Your session has expired. Please login again.');
            case 4:
              this.handleError(_t9);
            case 5:
              return _context9.a(2);
          }
        }, _callee9, this, [[0, 3]]);
      }));
      function getUser(_x8) {
        return _getUser.apply(this, arguments);
      }
      return getUser;
    }() // ✅ Add Product Report
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
  }, {
    key: "addProductReport",
    value: function () {
      var _addProductReport = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee0(request) {
        var token, body, response, _t0;
        return _regenerator().w(function (_context0) {
          while (1) switch (_context0.p = _context0.n) {
            case 0:
              _context0.p = 0;
              if (this.tokenStorage.isAuthorized()) {
                _context0.n = 1;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 1:
              token = this.tokenStorage.getAccessToken();
              body = {
                productBarID: request.productBarID || 0,
                message: request.message || '',
                reportType: request.reportType || '',
                readMe: request.readMe || '',
                readMeHtml: request.readMeHtml || '',
                files: (request.files || []).map(function (f) {
                  return {
                    fileBytes: f.fileBytes || '',
                    fileName: f.fileName || ''
                  };
                })
              };
              _context0.n = 2;
              return axios.post("".concat(this.API_BASE_URL, "/api/product/AddProductReport"), body, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Authorization': "Bearer ".concat(token),
                  'Content-Type': 'application/json'
                }
              });
            case 2:
              response = _context0.v;
              console.log("\uD83D\uDCDD Product report added for ProductBarID: ".concat(body.productBarID));
              return _context0.a(2, response.data);
            case 3:
              _context0.p = 3;
              _t0 = _context0.v;
              this.handleError(_t0);
            case 4:
              return _context0.a(2);
          }
        }, _callee0, this, [[0, 3]]);
      }));
      function addProductReport(_x9) {
        return _addProductReport.apply(this, arguments);
      }
      return addProductReport;
    }() // ✅ Add Rating
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
  }, {
    key: "addRating",
    value: function () {
      var _addRating = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee1(request) {
        var token, body, response, _t1;
        return _regenerator().w(function (_context1) {
          while (1) switch (_context1.p = _context1.n) {
            case 0:
              _context1.p = 0;
              if (this.tokenStorage.isAuthorized()) {
                _context1.n = 1;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 1:
              token = this.tokenStorage.getAccessToken();
              body = {
                productBarID: request.productBarID || 0,
                message: request.message || '',
                rateCount: request.rateCount || 0,
                files: (request.files || []).map(function (f) {
                  return {
                    fileBytes: f.fileBytes || '',
                    fileName: f.fileName || ''
                  };
                })
              };
              _context1.n = 2;
              return axios.post("".concat(this.API_BASE_URL, "/api/AddRating"), body, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Authorization': "Bearer ".concat(token),
                  'Content-Type': 'application/json'
                }
              });
            case 2:
              response = _context1.v;
              console.log("\u2B50 Rating added for ProductBarID: ".concat(body.productBarID));
              return _context1.a(2, response.data);
            case 3:
              _context1.p = 3;
              _t1 = _context1.v;
              this.handleError(_t1);
            case 4:
              return _context1.a(2);
          }
        }, _callee1, this, [[0, 3]]);
      }));
      function addRating(_x0) {
        return _addRating.apply(this, arguments);
      }
      return addRating;
    }() // ✅ Get Rating By Product
    /**
     * Retrieves rating(s) for a product by barID.
     * @param {Object|number} request - Either an object { barID } or a numeric barID
     * @returns {Promise<any>} rating response
     */
  }, {
    key: "getRatingByProduct",
    value: function () {
      var _getRatingByProduct = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee10(request) {
        var barID, response, _t10;
        return _regenerator().w(function (_context10) {
          while (1) switch (_context10.p = _context10.n) {
            case 0:
              _context10.p = 0;
              barID = typeof request === 'number' ? request : (request === null || request === void 0 ? void 0 : request.barID) || 0;
              _context10.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/public/GetRatingByProduct"), {
                barID: barID
              }, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 1:
              response = _context10.v;
              console.log("\u2B50 Ratings fetched for BarID: ".concat(barID));
              return _context10.a(2, response.data);
            case 2:
              _context10.p = 2;
              _t10 = _context10.v;
              this.handleError(_t10);
            case 3:
              return _context10.a(2);
          }
        }, _callee10, this, [[0, 2]]);
      }));
      function getRatingByProduct(_x1) {
        return _getRatingByProduct.apply(this, arguments);
      }
      return getRatingByProduct;
    }() // ✅ Create Checkout Session (Stripe)
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
  }, {
    key: "createCheckoutSession",
    value: function () {
      var _createCheckoutSession = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee11(request) {
        var body, headers, response, _t11;
        return _regenerator().w(function (_context11) {
          while (1) switch (_context11.p = _context11.n) {
            case 0:
              _context11.p = 0;
              body = {
                emailAddress: (request === null || request === void 0 ? void 0 : request.emailAddress) || '',
                addressBarID: (request === null || request === void 0 ? void 0 : request.addressBarID) || 0,
                items: ((request === null || request === void 0 ? void 0 : request.items) || []).map(function (i) {
                  return {
                    barId: i.barId || i.barID || 0,
                    slug: i.slug || '',
                    quantity: i.quantity || 1,
                    price: i.price || 0
                  };
                }),
                businessUnitKey: (request === null || request === void 0 ? void 0 : request.businessUnitKey) || this.businessUnitKey
              };
              headers = {
                'businessunitkey': this.businessUnitKey,
                'Content-Type': 'application/json'
              };
              if (this.tokenStorage.isAuthorized()) {
                headers.Authorization = "Bearer ".concat(this.tokenStorage.getAccessToken());
              }
              _context11.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/public/checkout/create-session"), body, {
                headers: headers
              });
            case 1:
              response = _context11.v;
              console.log("\uD83D\uDCB3 Checkout session created (businessUnitKey: ".concat(body.businessUnitKey, ")"));
              return _context11.a(2, response.data);
            case 2:
              _context11.p = 2;
              _t11 = _context11.v;
              this.handleError(_t11);
            case 3:
              return _context11.a(2);
          }
        }, _callee11, this, [[0, 2]]);
      }));
      function createCheckoutSession(_x10) {
        return _createCheckoutSession.apply(this, arguments);
      }
      return createCheckoutSession;
    }() // ✅ Send (simulate) Stripe webhook (testing helper)
    /**
     * Sends a raw Stripe webhook JSON payload to the API webhook receiver.
     * This is intended as a testing helper to simulate Stripe sending events.
     * @param {string|Object} payloadJson - Raw JSON string or object to send as the request body
     * @param {string} signature - The value for the "Stripe-Signature" header (for verification)
     * @param {string} [token] - Optional Bearer token if your webhook endpoint requires it (not typical)
     * @returns {Promise<any>} Response from webhook endpoint
     */
  }, {
    key: "sendStripeWebhook",
    value: function () {
      var _sendStripeWebhook = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee12(payloadJson, signature) {
        var body, headers, response, _error$response4, _t12;
        return _regenerator().w(function (_context12) {
          while (1) switch (_context12.p = _context12.n) {
            case 0:
              _context12.p = 0;
              body = typeof payloadJson === 'string' ? payloadJson : JSON.stringify(payloadJson);
              headers = {
                'businessunitkey': this.businessUnitKey,
                'Content-Type': 'application/json',
                'Stripe-Signature': signature || ''
              };
              if (this.tokenStorage.isAuthorized()) {
                headers.Authorization = "Bearer ".concat(this.tokenStorage.getAccessToken());
              }
              _context12.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/stripe/webhook"), body, {
                headers: headers
              });
            case 1:
              response = _context12.v;
              console.log('🔔 Stripe webhook simulated');
              return _context12.a(2, response.data);
            case 2:
              _context12.p = 2;
              _t12 = _context12.v;
              if (!(((_error$response4 = _t12.response) === null || _error$response4 === void 0 ? void 0 : _error$response4.status) === 401)) {
                _context12.n = 3;
                break;
              }
              this.tokenStorage.clearTokens();
              throw new Error('Unauthorized: Your session has expired. Please login again.');
            case 3:
              this.handleError(_t12);
            case 4:
              return _context12.a(2);
          }
        }, _callee12, this, [[0, 2]]);
      }));
      function sendStripeWebhook(_x11, _x12) {
        return _sendStripeWebhook.apply(this, arguments);
      }
      return sendStripeWebhook;
    }() // ✅ Get Product Details
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
  }, {
    key: "getProductDetails",
    value: function () {
      var _getProductDetails = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee13(request) {
        var body, response, _t13;
        return _regenerator().w(function (_context13) {
          while (1) switch (_context13.p = _context13.n) {
            case 0:
              _context13.p = 0;
              body = {
                slug: (request === null || request === void 0 ? void 0 : request.slug) || '',
                search: (request === null || request === void 0 ? void 0 : request.search) || '',
                sortBy: (request === null || request === void 0 ? void 0 : request.sortBy) || '',
                page: (request === null || request === void 0 ? void 0 : request.page) || 1,
                pageSize: (request === null || request === void 0 ? void 0 : request.pageSize) || 10
              };
              _context13.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/public/ProductDetails"), body, {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 1:
              response = _context13.v;
              console.log("\uD83D\uDD0E Product details fetched for slug: ".concat(body.slug));
              return _context13.a(2, response.data);
            case 2:
              _context13.p = 2;
              _t13 = _context13.v;
              this.handleError(_t13);
            case 3:
              return _context13.a(2);
          }
        }, _callee13, this, [[0, 2]]);
      }));
      function getProductDetails(_x13) {
        return _getProductDetails.apply(this, arguments);
      }
      return getProductDetails;
    }() // ✅ Get Report by Product
  }, {
    key: "getReportByProduct",
    value: function () {
      var _getReportByProduct = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee14(barID) {
        var response, _t14;
        return _regenerator().w(function (_context14) {
          while (1) switch (_context14.p = _context14.n) {
            case 0:
              _context14.p = 0;
              _context14.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/public/GetReportByProduct"), {
                barID: barID
              },
              // API expects this object
              {
                headers: {
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 1:
              response = _context14.v;
              console.log("\uD83D\uDCCB Retrieved reports for ProductBarID: ".concat(barID));
              return _context14.a(2, response.data);
            case 2:
              _context14.p = 2;
              _t14 = _context14.v;
              this.handleError(_t14);
            case 3:
              return _context14.a(2);
          }
        }, _callee14, this, [[0, 2]]);
      }));
      function getReportByProduct(_x14) {
        return _getReportByProduct.apply(this, arguments);
      }
      return getReportByProduct;
    }() // ✅ Update Product Report
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
  }, {
    key: "updateProductReport",
    value: function () {
      var _updateProductReport = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee15(request) {
        var token, requestBody, response, _t15;
        return _regenerator().w(function (_context15) {
          while (1) switch (_context15.p = _context15.n) {
            case 0:
              _context15.p = 0;
              if (this.tokenStorage.isAuthorized()) {
                _context15.n = 1;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 1:
              token = this.tokenStorage.getAccessToken(); // Ensure files array is initialized if not provided
              requestBody = {
                reportBarID: request.reportBarID || 0,
                message: request.message || '',
                reportType: request.reportType || '',
                readMe: request.readMe || '',
                readMeHtml: request.readMeHtml || '',
                files: (request.files || []).map(function (file) {
                  return {
                    fileBytes: file.fileBytes || '',
                    fileName: file.fileName || ''
                  };
                })
              };
              _context15.n = 2;
              return axios.put("".concat(this.API_BASE_URL, "/api/UpdateProductReport"), requestBody, {
                headers: {
                  'Authorization': "Bearer ".concat(token),
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 2:
              response = _context15.v;
              console.log("\uD83D\uDCDD Report updated successfully (ReportBarID: ".concat(request.reportBarID, ")"));
              return _context15.a(2, response.data);
            case 3:
              _context15.p = 3;
              _t15 = _context15.v;
              this.handleError(_t15);
            case 4:
              return _context15.a(2);
          }
        }, _callee15, this, [[0, 3]]);
      }));
      function updateProductReport(_x15) {
        return _updateProductReport.apply(this, arguments);
      }
      return updateProductReport;
    }() // ✅ GET CART (requires Bearer token)
  }, {
    key: "getCart",
    value: function () {
      var _getCart = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee16() {
        var token, response, _t16;
        return _regenerator().w(function (_context16) {
          while (1) switch (_context16.p = _context16.n) {
            case 0:
              _context16.p = 0;
              if (this.tokenStorage.isAuthorized()) {
                _context16.n = 1;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 1:
              token = this.tokenStorage.getAccessToken();
              _context16.n = 2;
              return axios.get("".concat(this.API_BASE_URL, "/api/product/GetCart"), {
                headers: {
                  'Authorization': "Bearer ".concat(token),
                  'businessunitkey': this.businessUnitKey
                }
              });
            case 2:
              response = _context16.v;
              console.log('🛒 Cart items retrieved successfully:');
              console.log(JSON.stringify(response.data, null, 2));
              return _context16.a(2, response.data);
            case 3:
              _context16.p = 3;
              _t16 = _context16.v;
              this.handleError(_t16);
            case 4:
              return _context16.a(2);
          }
        }, _callee16, this, [[0, 3]]);
      }));
      function getCart() {
        return _getCart.apply(this, arguments);
      }
      return getCart;
    }() // ✅ CLEAR CART
  }, {
    key: "clearCart",
    value: function () {
      var _clearCart = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee17() {
        var token, response, _t17;
        return _regenerator().w(function (_context17) {
          while (1) switch (_context17.p = _context17.n) {
            case 0:
              _context17.p = 0;
              if (this.tokenStorage.isAuthorized()) {
                _context17.n = 1;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 1:
              token = this.tokenStorage.getAccessToken();
              _context17.n = 2;
              return axios.post("".concat(this.API_BASE_URL, "/api/product/ClearCart"), {},
              // no body needed
              {
                headers: {
                  'Authorization': "Bearer ".concat(token),
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 2:
              response = _context17.v;
              console.log('🧹 Cart cleared successfully!');
              return _context17.a(2, response.status);
            case 3:
              _context17.p = 3;
              _t17 = _context17.v;
              this.handleError(_t17);
            case 4:
              return _context17.a(2);
          }
        }, _callee17, this, [[0, 3]]);
      }));
      function clearCart() {
        return _clearCart.apply(this, arguments);
      }
      return clearCart;
    }()
  }, {
    key: "getAboutInfo",
    value: function () {
      var _getAboutInfo = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee18() {
        var response, _t18;
        return _regenerator().w(function (_context18) {
          while (1) switch (_context18.p = _context18.n) {
            case 0:
              _context18.p = 0;
              _context18.n = 1;
              return axios.get("".concat(this.API_BASE_URL, "/api/public/AboutInfo"), {
                headers: {
                  "businessunitkey": this.businessUnitKey
                }
              });
            case 1:
              response = _context18.v;
              console.log("📘 About Info:", response.data);
              return _context18.a(2, response.data);
            case 2:
              _context18.p = 2;
              _t18 = _context18.v;
              this.handleError(_t18);
            case 3:
              return _context18.a(2);
          }
        }, _callee18, this, [[0, 2]]);
      }));
      function getAboutInfo() {
        return _getAboutInfo.apply(this, arguments);
      }
      return getAboutInfo;
    }() // ✅ Blogs
  }, {
    key: "getBlogs",
    value: function () {
      var _getBlogs = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee19() {
        var response, _t19;
        return _regenerator().w(function (_context19) {
          while (1) switch (_context19.p = _context19.n) {
            case 0:
              _context19.p = 0;
              _context19.n = 1;
              return axios.get("".concat(this.API_BASE_URL, "/api/public/GetBlogs"), {
                headers: {
                  "businessunitkey": this.businessUnitKey
                }
              });
            case 1:
              response = _context19.v;
              console.log("📰 Blogs:", response.data);
              return _context19.a(2, response.data);
            case 2:
              _context19.p = 2;
              _t19 = _context19.v;
              this.handleError(_t19);
            case 3:
              return _context19.a(2);
          }
        }, _callee19, this, [[0, 2]]);
      }));
      function getBlogs() {
        return _getBlogs.apply(this, arguments);
      }
      return getBlogs;
    }() // ✅ Documentation
  }, {
    key: "getDocumentation",
    value: function () {
      var _getDocumentation = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee20() {
        var response, _t20;
        return _regenerator().w(function (_context20) {
          while (1) switch (_context20.p = _context20.n) {
            case 0:
              _context20.p = 0;
              _context20.n = 1;
              return axios.get("".concat(this.API_BASE_URL, "/api/public/GetDocumentation"), {
                headers: {
                  "businessunitkey": this.businessUnitKey
                }
              });
            case 1:
              response = _context20.v;
              console.log("📄 Documentation:", response.data);
              return _context20.a(2, response.data);
            case 2:
              _context20.p = 2;
              _t20 = _context20.v;
              this.handleError(_t20);
            case 3:
              return _context20.a(2);
          }
        }, _callee20, this, [[0, 2]]);
      }));
      function getDocumentation() {
        return _getDocumentation.apply(this, arguments);
      }
      return getDocumentation;
    }() // ✅ Services
  }, {
    key: "getServices",
    value: function () {
      var _getServices = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee21() {
        var response, _t21;
        return _regenerator().w(function (_context21) {
          while (1) switch (_context21.p = _context21.n) {
            case 0:
              _context21.p = 0;
              _context21.n = 1;
              return axios.get("".concat(this.API_BASE_URL, "/api/public/GetServices"), {
                headers: {
                  "businessunitkey": this.businessUnitKey
                }
              });
            case 1:
              response = _context21.v;
              console.log("🛠️ Services:", response.data);
              return _context21.a(2, response.data);
            case 2:
              _context21.p = 2;
              _t21 = _context21.v;
              this.handleError(_t21);
            case 3:
              return _context21.a(2);
          }
        }, _callee21, this, [[0, 2]]);
      }));
      function getServices() {
        return _getServices.apply(this, arguments);
      }
      return getServices;
    }() // ✅ Get Address / Adres (no parameters)
    /**
     * Fetch address information from the API. Some backends may use "GetAddress" or "GetAdres".
     * This method tries both common routes: `/api/public/GetAddress` and `/api/public/GetAdres`.
     * It returns the first successful response.
     * @returns {Promise<any>} address data
     */
  }, {
    key: "getAddress",
    value: function () {
      var _getAddress = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee22() {
        var paths, _i, _paths, url, headers, response, _err$response, _t22;
        return _regenerator().w(function (_context22) {
          while (1) switch (_context22.p = _context22.n) {
            case 0:
              // Try the non-public routes first (matches API swagger screenshot)
              paths = ["".concat(this.API_BASE_URL, "/api/GetAddress")];
              _i = 0, _paths = paths;
            case 1:
              if (!(_i < _paths.length)) {
                _context22.n = 8;
                break;
              }
              url = _paths[_i];
              _context22.p = 2;
              headers = {
                businessunitkey: this.businessUnitKey
              };
              if (this.tokenStorage.isAuthorized()) {
                headers.Authorization = "Bearer ".concat(this.tokenStorage.getAccessToken());
              }
              _context22.n = 3;
              return axios.get(url, {
                headers: headers
              });
            case 3:
              response = _context22.v;
              console.log("\uD83D\uDCCD Address fetched from ".concat(url));
              return _context22.a(2, response.data);
            case 4:
              _context22.p = 4;
              _t22 = _context22.v;
              if (!(_t22.response && (_t22.response.status === 404 || _t22.response.status === 501))) {
                _context22.n = 5;
                break;
              }
              return _context22.a(3, 7);
            case 5:
              if (!(((_err$response = _t22.response) === null || _err$response === void 0 ? void 0 : _err$response.status) === 401)) {
                _context22.n = 6;
                break;
              }
              this.tokenStorage.clearTokens();
              throw new Error('Unauthorized: Your session has expired. Please login again.');
            case 6:
              this.handleError(_t22);
            case 7:
              _i++;
              _context22.n = 1;
              break;
            case 8:
              throw new Error('Address endpoint not found at GetAddress or GetAdres');
            case 9:
              return _context22.a(2);
          }
        }, _callee22, this, [[2, 4]]);
      }));
      function getAddress() {
        return _getAddress.apply(this, arguments);
      }
      return getAddress;
    }() // alias for getAddress using the spelling 'getAdres'
  }, {
    key: "getAdres",
    value: function () {
      var _getAdres = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee23() {
        return _regenerator().w(function (_context23) {
          while (1) switch (_context23.n) {
            case 0:
              return _context23.a(2, this.getAddress());
          }
        }, _callee23, this);
      }));
      function getAdres() {
        return _getAdres.apply(this, arguments);
      }
      return getAdres;
    }() // ✅ Upsert Address
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
  }, {
    key: "upsertAddress",
    value: function () {
      var _upsertAddress = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee24(request, token) {
        var _request$addressDetai, _request$addressDetai2, _request$addressDetai3, _request$addressDetai4, _request$addressDetai5, _request$addressDetai6, _request$addressDetai7, _request$addressDetai8, requestBody, response, _t23;
        return _regenerator().w(function (_context24) {
          while (1) switch (_context24.p = _context24.n) {
            case 0:
              _context24.p = 0;
              // Ensure all fields are present with defaults
              requestBody = {
                barID: request.barID || 0,
                addressDetails: {
                  barID: ((_request$addressDetai = request.addressDetails) === null || _request$addressDetai === void 0 ? void 0 : _request$addressDetai.barID) || request.barID || 0,
                  email: ((_request$addressDetai2 = request.addressDetails) === null || _request$addressDetai2 === void 0 ? void 0 : _request$addressDetai2.email) || '',
                  site: ((_request$addressDetai3 = request.addressDetails) === null || _request$addressDetai3 === void 0 ? void 0 : _request$addressDetai3.site) || '',
                  phoneNumber: ((_request$addressDetai4 = request.addressDetails) === null || _request$addressDetai4 === void 0 ? void 0 : _request$addressDetai4.phoneNumber) || '',
                  state: ((_request$addressDetai5 = request.addressDetails) === null || _request$addressDetai5 === void 0 ? void 0 : _request$addressDetai5.state) || '',
                  pincode: ((_request$addressDetai6 = request.addressDetails) === null || _request$addressDetai6 === void 0 ? void 0 : _request$addressDetai6.pincode) || '',
                  country: ((_request$addressDetai7 = request.addressDetails) === null || _request$addressDetai7 === void 0 ? void 0 : _request$addressDetai7.country) || '',
                  address: ((_request$addressDetai8 = request.addressDetails) === null || _request$addressDetai8 === void 0 ? void 0 : _request$addressDetai8.address) || ''
                }
              };
              _context24.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/UpsertAddress"), requestBody, {
                headers: {
                  'Authorization': "Bearer ".concat(token),
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 1:
              response = _context24.v;
              console.log("\uD83D\uDCCD Address upserted for BarID: ".concat(requestBody.barID));
              return _context24.a(2, response.data);
            case 2:
              _context24.p = 2;
              _t23 = _context24.v;
              this.handleError(_t23);
            case 3:
              return _context24.a(2);
          }
        }, _callee24, this, [[0, 2]]);
      }));
      function upsertAddress(_x16, _x17) {
        return _upsertAddress.apply(this, arguments);
      }
      return upsertAddress;
    }() // ✅ Add Products to Cart
    /**
     * Adds or updates products in the user's shopping cart
     * @param {Object[]} cartItems - Array of cart items to add
     * @param {string} cartItems[].slug - Product slug/identifier
     * @param {number} cartItems[].quantity - Quantity to add
     * @param {string} token - Bearer token for authentication
     * @returns {Promise<Object>} Response with success message
     */
  }, {
    key: "addProductsToCart",
    value: function () {
      var _addProductsToCart = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee25(cartItems) {
        var token, request, response, _t24;
        return _regenerator().w(function (_context25) {
          while (1) switch (_context25.p = _context25.n) {
            case 0:
              _context25.p = 0;
              if (this.tokenStorage.isAuthorized()) {
                _context25.n = 1;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 1:
              token = this.tokenStorage.getAccessToken();
              request = {
                cartItem: cartItems.map(function (item) {
                  return {
                    slug: item.slug || '',
                    quantity: item.quantity || 1
                  };
                })
              };
              _context25.n = 2;
              return axios.post("".concat(this.API_BASE_URL, "/api/product/AddProductsToCart"), request, {
                headers: {
                  'Authorization': "Bearer ".concat(token),
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 2:
              response = _context25.v;
              console.log('🛒 Products added to cart successfully');
              return _context25.a(2, response.data);
            case 3:
              _context25.p = 3;
              _t24 = _context25.v;
              this.handleError(_t24);
            case 4:
              return _context25.a(2);
          }
        }, _callee25, this, [[0, 3]]);
      }));
      function addProductsToCart(_x18) {
        return _addProductsToCart.apply(this, arguments);
      }
      return addProductsToCart;
    }() // ✅ Remove Product From Cart
    /**
     * Remove a product from the user's cart by slug.
     * @param {string} slug - product slug to remove
     * @param {string} token - Bearer token for authentication
     * @returns {Promise<Object|{status: number}>} API response or 204 status indicator
     */
  }, {
    key: "removeProductFromCart",
    value: function () {
      var _removeProductFromCart = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee26(slug) {
        var token, response, _t25;
        return _regenerator().w(function (_context26) {
          while (1) switch (_context26.p = _context26.n) {
            case 0:
              _context26.p = 0;
              if (this.tokenStorage.isAuthorized()) {
                _context26.n = 1;
                break;
              }
              throw new Error('Unauthorized: Please login first');
            case 1:
              token = this.tokenStorage.getAccessToken();
              _context26.n = 2;
              return axios.post("".concat(this.API_BASE_URL, "/api/product/RemoveProductFromTheCart"), {
                slug: slug
              }, {
                headers: {
                  'Authorization': "Bearer ".concat(token),
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 2:
              response = _context26.v;
              console.log("\uD83D\uDDD1\uFE0F Product removed from cart (slug: ".concat(slug, ")"));
              // Some endpoints return 204 No Content — normalize to a simple object
              return _context26.a(2, response.status === 204 ? {
                status: 204
              } : response.data);
            case 3:
              _context26.p = 3;
              _t25 = _context26.v;
              this.handleError(_t25);
            case 4:
              return _context26.a(2);
          }
        }, _callee26, this, [[0, 3]]);
      }));
      function removeProductFromCart(_x19) {
        return _removeProductFromCart.apply(this, arguments);
      }
      return removeProductFromCart;
    }() // ✅ Insert User Activity
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
  }, {
    key: "insertUserActivity",
    value: function () {
      var _insertUserActivity = _asyncToGenerator(/*#__PURE__*/_regenerator().m(function _callee27(request, token) {
        var body, response, _t26;
        return _regenerator().w(function (_context27) {
          while (1) switch (_context27.p = _context27.n) {
            case 0:
              _context27.p = 0;
              body = {
                userID: request.userID || '',
                businessUnitId: request.businessUnitId || request.businessUnitID || 0,
                eventType: request.eventType || '',
                metaData: request.metaData || ''
              };
              _context27.n = 1;
              return axios.post("".concat(this.API_BASE_URL, "/api/product/InsertUserActivity"), body, {
                headers: {
                  'Authorization': "Bearer ".concat(token),
                  'businessunitkey': this.businessUnitKey,
                  'Content-Type': 'application/json'
                }
              });
            case 1:
              response = _context27.v;
              console.log("\uD83D\uDCDD User activity inserted for userID: ".concat(body.userID));
              return _context27.a(2, response.data);
            case 2:
              _context27.p = 2;
              _t26 = _context27.v;
              this.handleError(_t26);
            case 3:
              return _context27.a(2);
          }
        }, _callee27, this, [[0, 2]]);
      }));
      function insertUserActivity(_x20, _x21) {
        return _insertUserActivity.apply(this, arguments);
      }
      return insertUserActivity;
    }() // Common error handler
  }, {
    key: "handleError",
    value: function handleError(error) {
      if (error.response) {
        console.error("API Error: ".concat(error.response.status));
        console.error(error.response.data);
        throw new Error("API Error: ".concat(error.response.status, " - ").concat(JSON.stringify(error.response.data)));
      } else if (error.request) {
        throw new Error("No response received: ".concat(error.message));
      } else {
        throw new Error("Error: ".concat(error.message));
      }
    }
  }]);
}();
export default ProductGroupClient;
//# sourceMappingURL=index.js.map