function _typeof(o) { "@babel/helpers - typeof"; return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (o) { return typeof o; } : function (o) { return o && "function" == typeof Symbol && o.constructor === Symbol && o !== Symbol.prototype ? "symbol" : typeof o; }, _typeof(o); }
function _classCallCheck(a, n) { if (!(a instanceof n)) throw new TypeError("Cannot call a class as a function"); }
function _defineProperties(e, r) { for (var t = 0; t < r.length; t++) { var o = r[t]; o.enumerable = o.enumerable || !1, o.configurable = !0, "value" in o && (o.writable = !0), Object.defineProperty(e, _toPropertyKey(o.key), o); } }
function _createClass(e, r, t) { return r && _defineProperties(e.prototype, r), t && _defineProperties(e, t), Object.defineProperty(e, "prototype", { writable: !1 }), e; }
function _toPropertyKey(t) { var i = _toPrimitive(t, "string"); return "symbol" == _typeof(i) ? i : i + ""; }
function _toPrimitive(t, r) { if ("object" != _typeof(t) || !t) return t; var e = t[Symbol.toPrimitive]; if (void 0 !== e) { var i = e.call(t, r || "default"); if ("object" != _typeof(i)) return i; throw new TypeError("@@toPrimitive must return a primitive value."); } return ("string" === r ? String : Number)(t); }
/**
 * TokenStorage class handles the storage and retrieval of authentication tokens.
 * Currently implements in-memory storage, but can be extended to use localStorage,
 * sessionStorage, or other storage mechanisms.
 */
var TokenStorage = /*#__PURE__*/function () {
  function TokenStorage() {
    _classCallCheck(this, TokenStorage);
    this.storage = {
      accessToken: null,
      refreshToken: null
    };
  }

  /**
   * Store both access and refresh tokens
   * @param {string} accessToken - The access token to store
   * @param {string} refreshToken - The refresh token to store
   */
  return _createClass(TokenStorage, [{
    key: "setTokens",
    value: function setTokens(accessToken, refreshToken) {
      this.storage.accessToken = accessToken;
      this.storage.refreshToken = refreshToken;
    }

    /**
     * Get the stored access token
     * @returns {string|null} The stored access token or null if not set
     */
  }, {
    key: "getAccessToken",
    value: function getAccessToken() {
      return this.storage.accessToken;
    }

    /**
     * Get the stored refresh token
     * @returns {string|null} The stored refresh token or null if not set
     */
  }, {
    key: "getRefreshToken",
    value: function getRefreshToken() {
      return this.storage.refreshToken;
    }

    /**
     * Check if the user is currently authorized (has an access token)
     * @returns {boolean} True if an access token is present
     */
  }, {
    key: "isAuthorized",
    value: function isAuthorized() {
      return !!this.storage.accessToken;
    }

    /**
     * Clear all stored tokens
     */
  }, {
    key: "clearTokens",
    value: function clearTokens() {
      this.storage.accessToken = null;
      this.storage.refreshToken = null;
    }
  }]);
}(); // Export a singleton instance to be used across the application
export var tokenStorage = new TokenStorage();

// Also export the class in case a new instance is needed
export { TokenStorage };
//# sourceMappingURL=tokenStorage.js.map