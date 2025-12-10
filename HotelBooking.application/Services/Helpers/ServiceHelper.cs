using System.Text.Json;
using HotelBooking.infrastructure.Models;

public static class ServiceHelper
{
    public static ServiceBaseDTO? MapToServiceDTO(Service sv)
    {
        switch ((ServiceTypeEnum)sv.ServiceTypeId)
        {
            case ServiceTypeEnum.Standard: // ID = 1
                return new ServiceStandardDTO
                {
                    Id = sv.Id,
                    Name = sv.Name,
                    Description = sv.Description,
                    Price = sv.Price,
                    IsDeleted = sv.IsDeleted,
                    ServiceTypeId = sv.ServiceTypeId,
                    // Map riêng
                    Unit = sv.GetStandardUnit()
                };

            case ServiceTypeEnum.AirportTransfer: // ID = 2
                var atData = sv.GetAirportData();
                return new ServiceAirportTransferDTO
                {
                    Id = sv.Id,
                    Name = sv.Name,
                    Description = sv.Description,
                    Price = sv.Price,
                    IsDeleted = sv.IsDeleted,
                    ServiceTypeId = sv.ServiceTypeId,
                    // Map riêng
                    MaxPassengers = atData?.MaxPassengers,
                    MaxLuggage = atData?.MaxLuggage,
                    RoundTripPrice = atData?.RoundTripPrice,
                    AdditionalFee = atData?.AdditionalFee,
                    AdditionalFeeStartTime = atData?.AdditionalFeeStartTime,
                    AdditionalFeeEndTime = atData?.AdditionalFeeEndTime
                };

            default:
                return null;
        }
    }

    // Dùng cho Create
    public static Service CreateObject(ServiceCreateOrUpdateDTO createDto)
    {
        return createDto switch
        {
            // Nếu là Standard DTO -> Gọi Factory Standard
            StdServiceCreateOrUpdateDTO std => Service.CreateStandard(
                std.Name,
                std.Description,
                0, // Giá mặc định khi tạo
                std.Unit
            ),

            // Nếu là Airport DTO -> Gọi Factory Airport
            AirportTransServiceCreateOrUpdateDTO at => Service.CreateAirportTransfer(
                at.Name,
                at.Description,
                0
            ),

            // THROW EXCEPTION: Để ServiceManage bên ngoài bắt
            _ => throw new ArgumentException("Loại DTO tạo mới không hợp lệ")
        };
    }

    // Dùng cho Update
    public static void UpdateObject(ServiceCreateOrUpdateDTO updateDto, Service entity)
    {
        // Kiểm tra tính hợp lệ: Không được đổi Type của dịch vụ đang có
        if (entity.ServiceTypeId != updateDto.TargetTypeId)
        {
            // THROW EXCEPTION: Ngừng luồng ngay lập tức
            throw new InvalidOperationException("Không thể thay đổi loại dịch vụ.");
        }

        switch (updateDto)
        {
            case StdServiceCreateOrUpdateDTO std:
                // Gọi hành vi Standard
                entity.ConfigureStandard(
                    std.Name,
                    std.Description,
                    entity.Price, // Giữ giá cũ
                    std.Unit
                );
                break;

            case AirportTransServiceCreateOrUpdateDTO at:
                // Cần map dữ liệu chi tiết từ DTO sang Model của Domain (nếu DTO có dữ liệu đó)
                // Giả sử DTO Update của bạn có các trường chi tiết (MaxPassengers...)
                // Ta map sang object AirportTransferDataModel để truyền vào Entity

                // 1. Tạo object Config (Model nội bộ của Infra)
                var dataModel = new AirportTransferDataModel
                {
                    // Map các trường từ at (DTO) sang dataModel (Domain POCO)
                    MaxPassengers = null,
                    MaxLuggage = null,
                    RoundTripPrice = null,
                    AdditionalFee = null,
                    AdditionalFeeStartTime = null,
                    AdditionalFeeEndTime = null
                };

                // Gọi hành vi Airport
                entity.ConfigureAirportTransfer(
                    at.Name,
                    at.Description,
                    entity.Price,
                    dataModel // Truyền object data vào, Entity tự serialize
                );
                break;

            default:
                throw new ArgumentException("Loại DTO không hợp lệ");
        }

    }
}