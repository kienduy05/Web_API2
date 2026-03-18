import flask
from config.db import get_connection

orderdetail_bp = flask.Blueprint("orderdetail_bp", __name__)

# GET ALL
@orderdetail_bp.route('/orderdetail/getall', methods=['GET'])
def get_all_orderdetail():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            SELECT OrderDetailID, OrderID, BookID, Quantity, UnitPrice
            FROM OrderDetail
        """)

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# GET BY ID
@orderdetail_bp.route('/orderdetail/getbyid/<id>', methods=['GET'])
def get_orderdetail_by_id(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            SELECT OrderDetailID, OrderID, BookID, Quantity, UnitPrice
            FROM OrderDetail
            WHERE OrderDetailID = ?
        """, (id,))

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# GET BY ORDER ID (Có kết hợp lấy thêm Tên Sách từ bảng Book)
@orderdetail_bp.route('/orderdetail/getbyorderid/<order_id>', methods=['GET'])
def get_orderdetail_by_order_id(order_id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        # Mình Join với bảng Book (theo sơ đồ của bạn) để lấy được BookName
        cursor.execute("""
            SELECT od.OrderDetailID, od.OrderID, od.BookID, b.BookName, od.Quantity, od.UnitPrice
            FROM OrderDetail od
            LEFT JOIN Book b ON od.BookID = b.BookID
            WHERE od.OrderID = ?
        """, (order_id,))

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# ADD
@orderdetail_bp.route('/orderdetail/add', methods=['POST'])
def add_orderdetail():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
        INSERT INTO OrderDetail (OrderID, BookID, Quantity, UnitPrice)
        VALUES (?, ?, ?, ?)
        """

        cursor.execute(command, (
            data["OrderID"],
            data["BookID"],
            data["Quantity"],
            data["UnitPrice"]
        ))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "them thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# UPDATE
@orderdetail_bp.route('/orderdetail/update', methods=['PUT'])
def update_orderdetail():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
        UPDATE OrderDetail
        SET OrderID=?, BookID=?, Quantity=?, UnitPrice=?
        WHERE OrderDetailID=?
        """

        cursor.execute(command, (
            data["OrderID"],
            data["BookID"],
            data["Quantity"],
            data["UnitPrice"],
            data["OrderDetailID"]
        ))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "cap nhat thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# DELETE
@orderdetail_bp.route('/orderdetail/delete/<id>', methods=['DELETE'])
def delete_orderdetail(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("DELETE FROM OrderDetail WHERE OrderDetailID = ?", (id,))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "xoa thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500