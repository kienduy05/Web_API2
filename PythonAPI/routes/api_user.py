import flask
from config.db import get_connection

user_bp = flask.Blueprint("user_bp", __name__)

@user_bp.route('/user/dashboard/<customer_id>', methods=['GET'])
def user_dashboard(customer_id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        # ===== CUSTOMER INFO =====
        cursor.execute("""
            SELECT CustomerID, CustomerName, CustomerPhone, CustomerEmail, CustomerAddress
            FROM Customer
            WHERE CustomerID = ?
        """, (customer_id,))
        customer = cursor.fetchone()
        customer_keys = [col[0] for col in cursor.description]
        customer_data = dict(zip(customer_keys, customer)) if customer else None

        # ===== ORDERS =====
        cursor.execute("""
            SELECT OrderID, OrderCreatedDate, OrderTotalAmount, OrderStatus
            FROM Orders
            WHERE CustomerID = ?
            ORDER BY OrderCreatedDate DESC
        """, (customer_id,))
        orders = []
        keys = [col[0] for col in cursor.description]
        for row in cursor.fetchall():
            orders.append(dict(zip(keys, row)))

        # ===== TOTAL SPENT =====
        cursor.execute("""
            SELECT ISNULL(SUM(OrderTotalAmount),0)
            FROM Orders
            WHERE CustomerID = ? AND OrderStatus = 2
        """, (customer_id,))
        total_spent = cursor.fetchone()[0]

        cursor.close()
        conn.close()

        return flask.jsonify({
            "customer": customer_data,
            "orders": orders,
            "totalSpent": float(total_spent)
        })

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

@user_bp.route('/user/update/<customer_id>', methods=['PUT'])
def update_user(customer_id):
    try:
        data = flask.request.json

        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            UPDATE Customer
            SET CustomerName = ?,
                CustomerPhone = ?,
                CustomerEmail = ?,
                CustomerAddress = ?
            WHERE CustomerID = ?
        """, (
            data["CustomerName"],
            data["CustomerPhone"],
            data["CustomerEmail"],
            data["CustomerAddress"],
            customer_id
        ))

        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"success": True, "mess": "Cập nhật thành công"})

    except Exception as e:
        return flask.jsonify({"success": False, "mess": str(e)}), 500

@user_bp.route('/user/change-password/<account_id>', methods=['PUT'])
def change_password(account_id):
    try:
        data = flask.request.json

        old_password = data.get("OldPassword")
        new_password = data.get("NewPassword")

        conn = get_connection()
        cursor = conn.cursor()

        # check password cũ
        cursor.execute("""
            SELECT Password FROM Account WHERE AccountID = ?
        """, (account_id,))
        row = cursor.fetchone()

        if not row:
            return flask.jsonify({"success": False, "mess": "Tài khoản không tồn tại"}), 404

        if row[0] != old_password:
            return flask.jsonify({"success": False, "mess": "Mật khẩu cũ không đúng"}), 400

        # update password mới
        cursor.execute("""
            UPDATE Account
            SET Password = ?
            WHERE AccountID = ?
        """, (new_password, account_id))

        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"success": True, "mess": "Đổi mật khẩu thành công"})

    except Exception as e:
        return flask.jsonify({"success": False, "mess": str(e)}), 500

@user_bp.route('/user/order-detail/<order_id>', methods=['GET'])
def user_order_detail(order_id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        # ===== ORDER INFO =====
        cursor.execute("""
            SELECT 
                OrderID,
                CustomerID,
                ReceiverName,
                ReceiverPhone,
                ReceiverAddress,
                OrderCreatedDate,
                OrderTotalAmount,
                OrderStatus
            FROM Orders
            WHERE OrderID = ?
        """, (order_id,))

        order_row = cursor.fetchone()

        if not order_row:
            return flask.jsonify({"success": False, "mess": "Không tìm thấy đơn hàng"}), 404

        order_keys = [col[0] for col in cursor.description]
        order = dict(zip(order_keys, order_row))

        # ===== ORDER DETAILS =====
        cursor.execute("""
            SELECT 
                od.OrderDetailID,
                od.BookID,
                b.BookName,
                od.Quantity,
                od.UnitPrice
            FROM OrderDetail od
            JOIN Book b ON od.BookID = b.BookID
            WHERE od.OrderID = ?
        """, (order_id,))

        details = []
        detail_keys = [col[0] for col in cursor.description]

        for row in cursor.fetchall():
            details.append(dict(zip(detail_keys, row)))

        cursor.close()
        conn.close()

        return flask.jsonify({
            "success": True,
            "order": order,
            "details": details
        })

    except Exception as e:
        return flask.jsonify({"success": False, "mess": str(e)}), 500