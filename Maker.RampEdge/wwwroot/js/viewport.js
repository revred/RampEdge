// Fire a callback once per element when it first enters the viewport (~30% visible)
// Usage from Blazor via JS interop: RampEdge.viewport.onceInView('.re-card', sku => DotNet.invokeMethodAsync(...))
window.RampEdge = window.RampEdge || {};
window.RampEdge.viewport = (function(){
  const seen = new WeakSet();
  let io;

  function ensureObserver(callback){
    if (io) return;
    io = new IntersectionObserver((entries) => {
      for (const e of entries) {
        if (e.isIntersecting && !seen.has(e.target)) {
          seen.add(e.target);
          const sku = e.target.getAttribute('data-sku');
          try {
            if (sku) callback(sku);
          } catch(err) {
            console.warn('Viewport callback error:', err);
          }
        }
      }
    }, { threshold: 0.3 });
  }

  function onceInView(selector, callback){
    ensureObserver(callback);
    document.querySelectorAll(selector).forEach(el => io.observe(el));
  }

  function cleanup() {
    if (io) {
      io.disconnect();
      io = null;
    }
  }

  return { onceInView, cleanup };
})();

// Initialize impression tracking for product cards
document.addEventListener('DOMContentLoaded', function(){
  if (window.RampEdge?.viewport) {
    window.RampEdge.viewport.onceInView('[data-sku]', function(sku){
      // Emit custom event for product impression
      window.dispatchEvent(new CustomEvent('product.impression', {
        detail: {
          sku: sku,
          timestamp: new Date().toISOString(),
          viewport: true
        }
      }));
    });
  }
});