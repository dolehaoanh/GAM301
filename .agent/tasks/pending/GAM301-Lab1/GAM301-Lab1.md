![](_page_0_Picture_1.jpeg)

# **Lab 1**

# **MỤC TIÊU**

Kết thúc bài thực hành sinh viên có khả năng:

- ✓ Hiểu cách sử dụng Coroutines để quản lý các sự kiện theo thời gian.
- ✓Điều khiển Coroutines dựa trên các điều kiện khác nhau.
- ✓Áp dụng Coroutines để thực hiện các thay đổi liên tục trong trò chơi.
- ✓Điều chỉnh thuộc tính vật liệu của đối tượng thông qua Coroutine.
- ✓Thực hiện tuần tự các hành động bằng cách sử dụng Coroutines.
- ✓Quản lý thời gian di chuyển đối tượng trong thế giới game.

### **NỘI DUNG**

### **[x] Bài tập 1: Sinh đối tượng sau một khoảng thời gian**

Tạo một script để sinh ra một đối tượng mới (như một khối lập phương) sau mỗi 2 giây tại các vị trí ngẫu nhiên trong một khu vực xác định. Sử dụng Coroutines để xử lý việc chờ giữa các lần sinh. Giới hạn số lượng đối tượng được sinh ra tối đa là 10.

Gợi ý:

Sử dụng hàm Instantiate() để tạo đối tượng mới. Sử dụng WaitForSeconds(2) trong Coroutine để chờ 2 giây. Dùng Random.Range() để tạo vị trí ngẫu nhiên trong khu vực xác định.

# **[x] Bài tập 2: Làm mờ dần một đối tượng theo thời gian**

#### Mô tả:

Tạo một script để làm mờ dần độ trong suốt của vật liệu của một đối tượng trong 5 giây khi người chơi nhấn phím space. Sử dụng Coroutine để điều chỉnh dần giá trị alpha của vật liệu theo thời gian.

Gợi ý:

![](_page_1_Picture_1.jpeg)

Sử dụng phím Input.GetKeyDown(KeyCode.Space) để bắt sự kiện nhấn phím. Sử dụng Material.color.a để điều chỉnh độ trong suốt (alpha) của vật liệu. Sử dụng WaitForEndOfFrame() hoặc WaitForSeconds() để làm giảm giá trị alpha từng chút một trong 5 giây.

## **[x] Bài tập 3: Di chuyển tuần tự nhiều đối tượng**

#### Mô tả:

Tạo một script để di chuyển 3 đối tượng đến các vị trí mới lần lượt, với thời gian chờ 1 giây giữa mỗi lần di chuyển. Sử dụng Coroutine để quản lý thời gian chờ và điều khiển di chuyển từng đối tượng.

#### Gợi ý:

Tạo 3 đối tượng và lưu chúng trong một mảng hoặc danh sách. Dùng transform.position để di chuyển từng đối tượng đến vị trí mới. Sử dụng WaitForSeconds(1) để chờ trước khi di chuyển đối tượng tiếp theo.

### **[x] Bài 4 : Giảng viên cho thêm**

## \*\*\* YÊU CẦU NỘP BÀI:

Sv nén file bao gồm các yêu cầu đã thực hiện trên, nộp lms đúng thời gian quy định của giảng viên. Không nộp bài coi như không có điểm.

--- Hết ---