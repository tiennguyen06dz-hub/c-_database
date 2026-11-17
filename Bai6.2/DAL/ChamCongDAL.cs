using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Bai6_2.DAL;

public class ChamCongDAL
{
    private DataSet? dataSet;
    private SqlDataAdapter? adapter;
    private SqlConnection? connection;
    private string thangDangNap = "";

    public DataTable NapChamCong(string thangNam)
    {
        connection?.Dispose();

        connection = DatabaseHelper.GetConnection();

        // Tạo dữ liệu chấm công cho nhân viên
        // chưa có bản ghi trong tháng.
        // Không tạo thêm khi tháng đã được chốt.
        connection.Open();

        using (var trans = connection.BeginTransaction(
            IsolationLevel.Serializable))
        {
            try
            {
                using var cmd = new SqlCommand("""
                    IF EXISTS
                    (
                        SELECT 1 FROM BangLuong
                        WHERE ThangNam=@ThangNam
                    )
                    BEGIN
                        SELECT 1;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO BangChamCong
                        (MaNV, ThangNam, SoNgayCong, SoGioOT)
                        SELECT nv.MaNV, @ThangNam, 0, 0
                        FROM NhanVien nv
                        WHERE NOT EXISTS
                        (
                            SELECT 1 FROM BangChamCong cc
                            WHERE cc.MaNV=nv.MaNV
                              AND cc.ThangNam=@ThangNam
                        );
                    END
                    """, connection, trans);

                cmd.Parameters.Add("@ThangNam",
                    SqlDbType.VarChar, 7).Value = thangNam;

                cmd.ExecuteNonQuery();
                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        connection.Close();

        adapter = new SqlDataAdapter("""
            SELECT MaCong, MaNV, ThangNam,
                   SoNgayCong, SoGioOT
            FROM BangChamCong
            WHERE ThangNam=@ThangNam
            """, connection);

        adapter.SelectCommand.Parameters.Add(
            "@ThangNam", SqlDbType.VarChar, 7)
            .Value = thangNam;

        adapter.MissingSchemaAction =
            MissingSchemaAction.AddWithKey;

        dataSet = new DataSet();
        adapter.Fill(dataSet, "ChamCong");

        // Sinh INSERT, UPDATE, DELETE bằng CommandBuilder.
        using var builder = new SqlCommandBuilder(adapter);
        adapter.InsertCommand = builder.GetInsertCommand(true);
        adapter.UpdateCommand = builder.GetUpdateCommand(true);
        adapter.DeleteCommand = builder.GetDeleteCommand(true);

        // Không cho sửa các cột định danh.
        var dt = dataSet.Tables["ChamCong"]!;
        dt.Columns["MaCong"]!.ReadOnly = true;
        dt.Columns["MaNV"]!.ReadOnly = true;
        dt.Columns["ThangNam"]!.ReadOnly = true;

        thangDangNap = thangNam;
        return dt;
    }

    public void LuuChamCong(string thangNam)
    {
        if (dataSet == null || adapter == null ||
            connection == null || thangNam != thangDangNap)
            throw new Exception(
                "Vui lòng nạp bảng chấm công trước.");

        var dt = dataSet.Tables["ChamCong"]!;

        foreach (DataRow row in dt.Rows)
        {
            if (row.RowState == DataRowState.Deleted)
                continue;

            double ngay = Convert.ToDouble(row["SoNgayCong"]);
            double ot = Convert.ToDouble(row["SoGioOT"]);

            if (ngay < 0 || ngay > 31 || ot < 0)
                throw new Exception(
                    "Ngày công phải từ 0 đến 31, OT không âm.");
        }

        connection.Open();

        using var trans = connection.BeginTransaction(
            IsolationLevel.Serializable);

        try
        {
            using (var check = new SqlCommand("""
                SELECT COUNT(*)
                FROM BangLuong WITH (UPDLOCK, HOLDLOCK)
                WHERE ThangNam=@ThangNam
                """, connection, trans))
            {
                check.Parameters.Add("@ThangNam",
                    SqlDbType.VarChar, 7).Value = thangNam;

                if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                    throw new Exception(
                        "Tháng này đã chốt lương, không thể sửa.");
            }

            adapter.InsertCommand!.Transaction = trans;
            adapter.UpdateCommand!.Transaction = trans;
            adapter.DeleteCommand!.Transaction = trans;

            adapter.Update(dt);

            trans.Commit();
            dt.AcceptChanges();
        }
        catch
        {
            trans.Rollback();
            throw;
        }
        finally
        {
            connection.Close();
        }
    }

    public bool DaChot(string thangNam)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new SqlCommand("""
            SELECT COUNT(*) FROM BangLuong
            WHERE ThangNam=@ThangNam
            """, conn);

        cmd.Parameters.Add("@ThangNam",
            SqlDbType.VarChar, 7).Value = thangNam;

        conn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }
}