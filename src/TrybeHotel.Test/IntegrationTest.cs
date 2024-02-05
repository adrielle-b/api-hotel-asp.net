namespace TrybeHotel.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Diagnostics;
using System.Xml;
using System.IO;
using TrybeHotel.Dto;

public class IntegrationTest: IClassFixture<WebApplicationFactory<Program>>
{
     public HttpClient _clientTest;

     public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDatabase");
                });
                services.AddScoped<ITrybeHotelContext, ContextTest>();
                services.AddScoped<ICityRepository, CityRepository>();
                services.AddScoped<IHotelRepository, HotelRepository>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();
                    appContext.Cities.Add(new City {CityId = 1, Name = "Manaus", State = "AM"});
                    appContext.Cities.Add(new City {CityId = 2, Name = "Palmas", State = "TO"});
                    appContext.SaveChanges();
                    appContext.Hotels.Add(new Hotel {HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1});
                    appContext.Hotels.Add(new Hotel {HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2});
                    appContext.Hotels.Add(new Hotel {HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1});
                    appContext.SaveChanges();
                    appContext.Rooms.Add(new Room { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 2, Name = "Room 2", Capacity = 3, Image = "Image 2", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 3, Name = "Room 3", Capacity = 4, Image = "Image 3", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 4, Name = "Room 4", Capacity = 2, Image = "Image 4", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 5, Name = "Room 5", Capacity = 3, Image = "Image 5", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 6, Name = "Room 6", Capacity = 4, Image = "Image 6", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 7, Name = "Room 7", Capacity = 2, Image = "Image 7", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 8, Name = "Room 8", Capacity = 3, Image = "Image 8", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 9, Name = "Room 9", Capacity = 4, Image = "Image 9", HotelId = 3 });
                    appContext.SaveChanges();
                    appContext.Users.Add(new User { UserId = 1, Name = "Ana", Email = "ana@trybehotel.com", Password = "Senha1", UserType = "admin" });
                    appContext.Users.Add(new User { UserId = 2, Name = "Beatriz", Email = "beatriz@trybehotel.com", Password = "Senha2", UserType = "client" });
                    appContext.Users.Add(new User { UserId = 3, Name = "Laura", Email = "laura@trybehotel.com", Password = "Senha3", UserType = "client" });
                    appContext.SaveChanges();
                    appContext.Bookings.Add(new Booking { BookingId = 1, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 2, RoomId = 1});
                    appContext.Bookings.Add(new Booking { BookingId = 2, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 3, RoomId = 4});
                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }
    public class LoginResponse {
        public string? token { get; set; }
    }
    public class ErrorResponse {
        public string? message { get; set; }
    }

    public StringContent convertBody(object objectToBody)
    {
        return new StringContent (
            JsonConvert.SerializeObject(objectToBody),
            System.Text.Encoding.UTF8,
            "application/json"
        );
    }
    public async Task<T> convertResponseType<T>(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseString);
    }
    public async Task<string> GetToken(string role)
    {   
        if (role == "admin") {
        var loginAdmin = new {
            Email = "ana@trybehotel.com",
            Password = "Senha1"
        };
        var response = await _clientTest.PostAsync("/login", convertBody(loginAdmin));
        
        var admin = await convertResponseType<LoginResponse>(response);
        return admin.token!;
        }
        if (role == "client") {
            var loginClient = new {
            Email = "beatriz@trybehotel.com",
            Password = "Senha2"
        };
        var response = await _clientTest.PostAsync("/login", convertBody(loginClient));
        
        var client = await convertResponseType<LoginResponse>(response);
        return client.token!;
        }
        return "";
    }

    public class CityPost {
        public string? Name { get; set; }
    }
    public class HotelPost {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int CityId { get; set; }
    }
    public class RoomPost {
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string? Image { get; set; }
        public int HotelId { get; set; }
    }

    // USER
    [Trait("Category", "Teste endpoint /user")]
    [Theory(DisplayName = "GET /user")]
    [InlineData("/user")]
    public async Task TestGetUsers(string url)
    {   
        var token = await GetToken("admin");
        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await _clientTest.GetAsync(url);
        var responseUser = await convertResponseType<List<UserDto>>(response);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, responseUser?.Count);     
    }

    [Trait("Category", "Teste endpoint /user")]
    [Theory(DisplayName = "POST /user")]
    [InlineData("/user")]
    public async Task TestAddUser(string url)
    {   
        var newUser = new {
            Name = "Pedro",
            Email = "pedro@email.com",
            Password = "Senha4",
        };
        var response = await _clientTest.PostAsync(url, convertBody(newUser));
        var responseUser = await convertResponseType<User>(response);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Pedro", responseUser?.Name);
        Assert.Equal("client", responseUser?.UserType);    
    }

    // LOGIN
    [Trait("Category", "Teste endpoint /login")]
    [Theory(DisplayName = "POST /login")]
    [InlineData("/login")]
    public async Task TestLogin(string url)
    {
        var login = new {
            Email = "ana@trybehotel.com",
            Password = "Senha1"
        };
        var response = await _clientTest.PostAsync(url, convertBody(login));
        var responseLogin = await convertResponseType<LoginResponse>(response);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseLogin.token);
    }

    [Trait("Category", "Teste endpoint /login")]
    [Theory(DisplayName = "POST /login - Unauthorized")]
    [InlineData("/login")]
    public async Task TestLoginUnauthorized(string url)
    {
        var login = new {
            Email = "ana@trybehotel.com",
            Password = "Senha12"
        };
        var response = await _clientTest.PostAsync(url, convertBody(login));
        var responseLogin = await convertResponseType<ErrorResponse>(response);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Incorrect e-mail or password", responseLogin.message);
    }

    // ROOM
    [Trait("Category", "Teste endpoint /room")]
    [Theory(DisplayName = "POST /room")]
    [InlineData("/room")]
    public async Task TestPostRoom(string url)
    {
         var token = await GetToken("admin");
        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var newRoom = new RoomPost { 
            Name = "Room 10", 
            Capacity = 2, 
            Image = "Image 10", 
            HotelId = 1 
        };
     
        var response = await _clientTest.PostAsync(url, convertBody(newRoom));
        var responseRoom = await convertResponseType<RoomDto>(response);


        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Room 10", responseRoom.name);
        Assert.Equal(2, responseRoom.capacity);
    }

    [Trait("Category", "Teste endpoint /room")]
    [Theory(DisplayName = "GET /room")]
    [InlineData("/room/1")]
    public async Task TestGetRoom(string url)
    {
        var response = await _clientTest.GetAsync(url);
        var responseRoom = await convertResponseType<List<RoomDto>>(response);
        var room = responseRoom.Find(r => r.name == "Room 1");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(9, responseRoom?.Count);
        Assert.Equal(1, room?.roomId);
    }

    [Trait("Category", "Teste endpoint /room")]
    [Theory(DisplayName = "DELETE /room")]
    [InlineData("/room/1")]
    public async Task TestDeleteRoom(string url)
    {
        var token = await GetToken("admin");
        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.DeleteAsync(url);

        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    // CITY
    [Trait("Category", "Teste endpoint /city")]
    [Theory(DisplayName = "POST /city")]
    [InlineData("/city")]
    public async Task TestPostCity(string url)
    {
        var newCity = new CityPost { Name = "São Paulo" };

        var response = await _clientTest.PostAsync(url, convertBody(newCity));
        var responseCity = await convertResponseType<CityDto>(response);
       
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("São Paulo", responseCity.name);
    }
   
    [Trait("Category", "Teste endpoint /city")]
    [Theory(DisplayName = "GET /city")]
    [InlineData("/city")]
    public async Task TestGetCities(string url)
    {
        var response = await _clientTest.GetAsync(url);
        var responseCity = await convertResponseType<List<CityDto>>(response);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, responseCity.Count);
    }

    // HOTEL
    [Trait("Category", "Teste endpoint /hotel")]
    [Theory(DisplayName = "GET /hotel")]
    [InlineData("/hotel")]
    public async Task TestGetHotels(string url)
    {
        var response = await _clientTest.GetAsync(url);
        var responseHotels = await convertResponseType<List<HotelDto>>(response);
       
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, responseHotels?.Count);       
    }

    [Trait("Category", "Teste endpoint /hotel")]
    [Theory(DisplayName = "POST /hotel")]
    [InlineData("/hotel")]
    public async Task TestPostHotel(string url)
    {
        var token = await GetToken("admin");
        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var newHotel = new HotelPost { 
            Name = "Hotel Rio de Janeiro", 
            Address = "Address 4", 
            CityId = 3 
        };

        var response = await _clientTest.PostAsync(url, convertBody(newHotel));
        var responseHotel = await convertResponseType<HotelDto>(response);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Hotel Rio de Janeiro", responseHotel.name);
    }

    // BOOKING
    [Trait("Category", "Teste endpoint /booking")]
    [Theory(DisplayName = "GET /booking")]
    [InlineData("/booking/1")]
    public async Task TestGetBookings(string url)
    {
        var token = await GetToken("client");
        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await _clientTest.GetAsync(url);
        var responseBooking = await response.Content.ReadAsStringAsync();
        var booking = JsonConvert.DeserializeObject<Booking>(responseBooking);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, booking?.BookingId);
    }

}