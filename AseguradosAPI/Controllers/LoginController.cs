using AseguradosAPI.Data;
using AseguradosAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AseguradosAPI.Controllers
{
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public LoginController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // Acción GET para mostrar el formulario de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Users model)
        {
            try
            {
                var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserName == model.UserName);
                // Validar usuario
                if (user != null && user.UserName == model.UserName && user.UserPassword == model.UserPassword)
                {
                    var token = GenerateJwtToken(model.UserName);

                    // Guardar el token en una cookie
                    Response.Cookies.Append("AuthToken", token, new CookieOptions
                    {
                        HttpOnly = true, // No accesible desde JS
                        Secure = true, // Asegúrate de que la cookie sea solo para HTTPS en producción
                        SameSite = SameSiteMode.Strict, // Ayuda a prevenir ataques CSRF
                        Expires = DateTime.Now.AddMinutes(30) // Tiempo de expiración
                    });

                    return RedirectToAction("Index", "Employees");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                }


                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool ValidateUser(string username, string password)
        {
            // Valida las credenciales del usuario
            return username == "user" && password == "password";
        }

        // Acción para cerrar sesión (Logout)
        public IActionResult Logout()
        {
            // Eliminar la cookie que contiene el JWT
            Response.Cookies.Delete("AuthToken");

            // Redireccionar al login o a una página de tu elección
            return RedirectToAction("Login");
        }
    }
}