using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AseguradosAPI.Data;
using AseguradosAPI.Models;
using Microsoft.Data.SqlClient;

namespace AseguradosAPI.Controllers
{
    [Route("[controller]/[action]")]
    public class BeneficiariesController : Controller
    {
        private readonly AppDbContext _context;

        public BeneficiariesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Beneficiaries
        public async Task<IActionResult> Index()
        {
            var res = _context.Beneficiaries.Include(b => b.Employee);
            var lst = res.ToList();

            var lstBeneficiarios = new List<Beneficiary>();
            foreach (var item in lst)
            {
                var beneficiario = new Beneficiary();
                beneficiario.BeneficiaryId = item.BeneficiaryId;
                beneficiario.FirstName = item.FirstName;
                beneficiario.LastName = item.LastName;
                beneficiario.BirthDate = item.BirthDate;
                beneficiario.CURP = item.CURP;
                beneficiario.SSN = item.SSN;
                beneficiario.Phone = item.Phone;
                beneficiario.ParticipationPercentage = item.ParticipationPercentage;
                beneficiario.Nationality = item.Nationality;
                beneficiario.EmployeeId = item.EmployeeId;
                lstBeneficiarios.Add(beneficiario);
            }

            return View(await _context.Beneficiaries.ToListAsync());
            //return View(await appDbContext.ToListAsync());
        }

        // GET: Beneficiaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beneficiary = await _context.Beneficiaries
                .Include(b => b.Employee)
                .FirstOrDefaultAsync(m => m.BeneficiaryId == id);
            if (beneficiary == null)
            {
                return NotFound();
            }

            return View(beneficiary);
        }

        // GET: Beneficiaries/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "CURP");
            return View();
        }

        // POST: Beneficiaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BeneficiaryId,FirstName,LastName,BirthDate,CURP,SSN,Phone,Nationality,ParticipationPercentage,EmployeeId")] Beneficiary beneficiary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(beneficiary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "CURP", beneficiary.EmployeeId);
            return View(beneficiary);
        }

        // GET: Beneficiaries/EditBeneficiaries/5
        public async Task<IActionResult> EditBeneficiaries(int id)
        {
            // Obtener la lista de beneficiarios del empleado específico
            var beneficiaries = await _context.Beneficiaries
                .Where(b => b.EmployeeId == id)
                .ToListAsync();

            // Inicializa el ViewModel con el EmployeeId y la lista de Beneficiaries
            var model = new EditBeneficiariesViewModel
            {
                EmployeeId = id,
                Beneficiaries = beneficiaries ?? new List<Beneficiary>()
            };

            return View(model);
        }

        // POST: Beneficiaries/EditBeneficiaries
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBeneficiaries(EditBeneficiariesViewModel model)
        {
            if (model != null)
            {
                // Validar que la suma de las participaciones no supere el 100%
                var totalParticipation = model.Beneficiaries.Sum(b => b.ParticipationPercentage);

                if (totalParticipation != 100)
                {
                    // Obtener la lista de beneficiarios del empleado específico
                    var beneficiaries = await _context.Beneficiaries
                        .Where(b => b.EmployeeId == model.EmployeeId)
                        .ToListAsync();
                    ModelState.AddModelError( "", "La suma de las participaciones debe ser igual al 100%.");

                    return View(model);
                }

                // Actualizar los beneficiarios en la base de datos
                foreach (var beneficiary in model.Beneficiaries)
                {
                   await  UpdateBenefic(beneficiary);
                }
                
                return RedirectToAction(nameof(Index));  // Redirigir a la página principal después de guardar
            }

            return View(model);  // Retornar a la vista en caso de error en el modelo
        }

        public async Task<bool> UpdateBenefic(Beneficiary model)
        {
            try
            {
                var sql = "EXEC UpdateBeneficiary @BeneficiaryId, @FirstName, @LastName, @BirthDate, @CURP, @SSN, @Phone, @Nationality, @ParticipationPercentage";

                // Ejecuta el procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync(sql,
                    new SqlParameter("@BeneficiaryId", model.BeneficiaryId),
                    new SqlParameter("@FirstName", model.FirstName ?? (object)DBNull.Value),
                    new SqlParameter("@LastName", model.LastName ?? (object)DBNull.Value),
                    new SqlParameter("@BirthDate", model.BirthDate),
                    new SqlParameter("@CURP", model.CURP ?? (object)DBNull.Value),
                    new SqlParameter("@SSN", model.SSN ?? (object)DBNull.Value),
                    new SqlParameter("@Phone", model.Phone ?? (object)DBNull.Value),
                    new SqlParameter("@Nationality", model.Nationality ?? (object)DBNull.Value),
                    new SqlParameter("@ParticipationPercentage", model.ParticipationPercentage)
                );
                return true;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error al actualizar el beneficiario " + model.FirstName + " "  + model.LastName, e );
            }
        }


        // GET: Beneficiaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beneficiary = await _context.Beneficiaries
                .Include(b => b.Employee)
                .FirstOrDefaultAsync(m => m.BeneficiaryId == id);
            if (beneficiary == null)
            {
                return NotFound();
            }

            return View(beneficiary);
        }

        // POST: Beneficiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var beneficiary = await _context.Beneficiaries.FindAsync(id);
            if (beneficiary != null)
            {
                _context.Beneficiaries.Remove(beneficiary);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeneficiaryExists(int id)
        {
            return _context.Beneficiaries.Any(e => e.BeneficiaryId == id);
        }
    }
}
