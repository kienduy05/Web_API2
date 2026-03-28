const checkoutService = {
    createOrder: async (orderData) => {
        const response = await fetch('/Checkout/CreateOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(orderData)
        });
        return response;
    }
};
