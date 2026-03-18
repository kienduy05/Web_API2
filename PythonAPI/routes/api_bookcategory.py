import flask
from config.db import get_connection

category_bp = flask.Blueprint("category_bp", __name__)

# 1. GET ALL
@category_bp.route('/bookcategory/getall', methods=['GET'])
def get_all_category():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM BookCategory")

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()
        return flask.jsonify(results)
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 2. GET BY ID
@category_bp.route('/bookcategory/getbyid/<id>', methods=['GET'])
def get_category_by_id(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM BookCategory WHERE BookCategoryID = ?", (id,))

        keys = [col[0] for col in cursor.description]
        row = cursor.fetchone()

        cursor.close()
        conn.close()

        if row:
            return flask.jsonify(dict(zip(keys, row)))
        return flask.jsonify({"mess": "Khong tim thay danh muc"}), 404
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 3. INSERT
@category_bp.route('/bookcategory/add', methods=['POST'])
def add_category():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = "INSERT INTO BookCategory (BookCategoryName) VALUES (?)"
        params = (data.get("BookCategoryName"),)

        cursor.execute(command, params)
        conn.commit()

        cursor.close()
        conn.close()
        return flask.jsonify({"mess": "Them danh muc thanh cong"}), 201
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 4. UPDATE
@category_bp.route('/bookcategory/update', methods=['PUT'])
def update_category():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
                  UPDATE BookCategory
                  SET BookCategoryName = ?
                  WHERE BookCategoryID = ?
                  """
        params = (data["BookCategoryName"], data["BookCategoryID"])

        cursor.execute(command, params)
        conn.commit()

        cursor.close()
        conn.close()
        return flask.jsonify({"mess": "Cap nhat danh muc thanh cong"})
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 5. DELETE
@category_bp.route('/bookcategory/delete/<id>', methods=['DELETE'])
def delete_category(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("DELETE FROM BookCategory WHERE BookCategoryID = ?", (id,))
        conn.commit()

        cursor.close()
        conn.close()
        return flask.jsonify({"mess": "Xoa danh muc thanh cong"})
    except Exception as e:
        return flask.jsonify({"mess": "Khong the xoa vi danh muc dang duoc su dung", "error": str(e)}), 400