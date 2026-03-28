const cartService = {
    removeFromCart: async (bookId) => {
        const response = await fetch(`/Cart/RemoveFromCart`, { 
            method: 'POST', 
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ bookId: bookId })
        });
        return await response.json();
    },
    
    updateQuantity: async (bookId, quantity) => {
        const response = await fetch(`/Cart/UpdateQuantity/${bookId}`, { 
            method: 'POST', 
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ quantity })
        });
        return await response.json();
    }
};
