import flask
from config.db import get_connection

orders_bp = flask.Blueprint("orders_bp", __name__)

# GET ALL
@orders_bp.route('/orders/getall', methods=['GET'])
def get_all_orders():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            SELECT OrderID, CustomerID, OrderCreatedDate, ReceiverName, 
                   ReceiverPhone, ReceiverAddress, OrderTotalAmount, OrderStatus
            FROM Orders
        """)

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# GET BY ID
@orders_bp.route('/orders/getbyid/<id>', methods=['GET'])
def get_orders_by_id(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            SELECT OrderID, CustomerID, OrderCreatedDate, ReceiverName, 
                   ReceiverPhone, ReceiverAddress, OrderTotalAmount, OrderStatus
            FROM Orders
            WHERE OrderID = ?
        """, (id,))

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# GET BY CUSTOMER ID
@orders_bp.route('/orders/getbycustomerid/<customer_id>', methods=['GET'])
def get_orders_by_customer_id(customer_id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            SELECT OrderID, CustomerID, OrderCreatedDate, ReceiverName, 
                   ReceiverPhone, ReceiverAddress, OrderTotalAmount, OrderStatus
            FROM Orders
            WHERE CustomerID = ?
        """, (customer_id,))

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# ADD
@orders_bp.route('/orders/add', methods=['POST'])
def add_orders():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
        INSERT INTO Orders (CustomerID, OrderCreatedDate, ReceiverName, ReceiverPhone, ReceiverAddress, OrderTotalAmount, OrderStatus)
        VALUES (?, ?, ?, ?, ?, ?, ?)
        """

        cursor.execute(command, (
            data["CustomerID"],
            data["OrderCreatedDate"],
            data["ReceiverName"],
            data["ReceiverPhone"],
            data["ReceiverAddress"],
            data["OrderTotalAmount"],
            data["OrderStatus"]
        ))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "them thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# UPDATE
@orders_bp.route('/orders/update', methods=['PUT'])
def update_orders():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
        UPDATE Orders
        SET CustomerID=?, OrderCreatedDate=?, ReceiverName=?, ReceiverPhone=?, 
            ReceiverAddress=?, OrderTotalAmount=?, OrderStatus=?
        WHERE OrderID=?
        """

        cursor.execute(command, (
            data["CustomerID"],
            data["OrderCreatedDate"],
            data["ReceiverName"],
            data["ReceiverPhone"],
            data["ReceiverAddress"],
            data["OrderTotalAmount"],
            data["OrderStatus"],
            data["OrderID"]
        ))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "cap nhat thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# DELETE
@orders_bp.route('/orders/delete/<id>', methods=['DELETE'])
def delete_orders(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("DELETE FROM Orders WHERE OrderID = ?", (id,))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "xoa thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500