from operator import index
import qrcode


index = 0
count = 27
while(count>index):
    QR_Data = qrcode.QRCode(
                version=1,
                box_size=10,
                border=8)

    QR_Data.add_data(str(index))
    QR_Data.make(fit=True)
    img = QR_Data.make_image(fill='black', back_color='white')
    img.save("Test_"+str(index)+".jpg")
    index+=1